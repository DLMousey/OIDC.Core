// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;

namespace OIDC.Core_Provision;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            string versionString = Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion
                .ToString();
        
            Console.WriteLine($"oidc.core_provision v{versionString}");
            Console.WriteLine("---------------");
            Console.WriteLine("Usage: oidc.core-provision <admin-username> <initial-app-name>");
            return;
        }

        string username = args[0];
        string appName = args[1];
        string apiPath = args[2];
        
        Console.WriteLine("Received arguments:");
        Console.WriteLine("Username: " + username);
        Console.WriteLine("App name: " + appName);
        Console.WriteLine("---------------");
        Console.WriteLine("Gathering facts...");

        if (!Directory.Exists(apiPath))
        {
            Console.WriteLine("Invalid directory provided for API path - please ensure this is an absolute path to where the OIDC.Core-API .csproj is located on your system");
        }

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(Path.Join(apiPath, "appsettings.Development.json"), optional: false, reloadOnChange: false)
            .AddJsonFile(Path.Join(apiPath, "/oidc-core.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        ServiceCollection collection = new ServiceCollection();
        collection.AddTransient<IUserService, UserService>();
        collection.AddTransient<IApplicationService, ApplicationService>();
        collection.AddTransient<IScopeService, ScopeService>();
        collection.AddTransient<IRoleService, RoleService>();
        collection.AddTransient<IRandomValueService, RandomValueService>();
        collection.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            config.GetConnectionString("DefaultConnection")!)
        );

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        IServiceProvider serviceProvider = collection.BuildServiceProvider();
        
        Console.WriteLine("Service & config providers built successfully");

        IUserService userService = serviceProvider.GetService<IUserService>()!;
        IApplicationService applicationService = serviceProvider.GetService<IApplicationService>()!;
        IRandomValueService randomValueService = new RandomValueService();
        AppDbContext context = serviceProvider.GetService<AppDbContext>()!;

        CreateRoles(context);
        CreateScopes(context);
        
        string password = randomValueService.CryptoSafeRandomString(12);
        User user = CreateUser(userService, randomValueService, username, password);
        Application application = CreateApplication(applicationService, context, appName, user);

        AssignRoles(context, user);

        Console.WriteLine("\nProvisioning complete");
        Console.WriteLine("---------------");
        
        Console.WriteLine($"Initial user email: {user.Email}");
        Console.WriteLine($"Initial user password: {password}");
        Console.WriteLine($"Initial app client id: {application.ClientId}");
        Console.WriteLine($"Initial app client secret: {application.ClientSecret}");
    }

    private static void CreateRoles(AppDbContext context)
    {
        Console.WriteLine("Creating initial roles...");

        List<Role> roles = new List<Role>();
        roles.Add(new Role
        {
            Name = "User",
            UserRoles = new List<UserRole>()
        });
        roles.Add(new Role
        {
            Name = "Admin",
            UserRoles = new List<UserRole>()
        });
        
        roles.ForEach(r => context.Roles.Add(r));
        context.SaveChanges();
        
        Console.WriteLine("Initial roles created successfully");
    }

    private static void CreateScopes(AppDbContext context)
    {
        Console.WriteLine("Creating initial scopes...");

        List<Scope> scopes = new List<Scope>();
        scopes.Add(new Scope
        {
            Name = "profile.read", 
            Description = "Read information from your profile such as username, email address, etc",
            Dangerous = false,
            Label = "Read your profile information",
            Icon = "todo"
        });
        scopes.Add(new Scope
        {
            Name = "profile.write",
            Description = "Add/update information on your profile",
            Dangerous = true,
            Label = "Change your profile information",
            Icon = "todo"
        });
        scopes.Add(new Scope
        {
            Name = "scopes.list",
            Description = "List available scopes",
            Dangerous = true,
            Label = "List all the available scopes on the platform, including admin scopes. Available to admin accounts only",
            Icon = "todo"
        });
        scopes.Add(new Scope
        {
            Name = "scopes.create",
            Description = "Create new scopes",
            Dangerous = true,
            Label = "Create new OAuth scopes available for use on the platform. Available to admin accounts only",
            Icon = "todo"
        });
        
        scopes.ForEach(s => context.Scopes.Add(s));
        context.SaveChanges();
        
        Console.WriteLine("Initial scopes created successfully: profile.read, profile.write");
    }

    private static Application CreateApplication(IApplicationService applicationService, AppDbContext context, string appName, User user)
    {
        Console.WriteLine("Building initial oauth application...");
        
        CreateRequestViewModel applicationVm = new CreateRequestViewModel
        {
            Name = appName,
            Description = "Provisioned by OIDC.Core-Provision",
            HomepageUrl = "http://localhost:4200",
            RedirectUrl = "http://localhost:4200/oauth-return"
        };

        Application application = applicationService.CreateAsync(applicationVm, user).Result;
        application.FirstParty = true;
        
        context.Applications.Update(application);
        context.SaveChanges();

        Console.WriteLine($"Initial oauth application created successfully, GUID: {application.Id}");

        return application;
    }

    private static User CreateUser(IUserService userService, IRandomValueService randomValueService, string username, string password)
    {
        Console.WriteLine("Building initial user... (password will be displayed at end of provisioning)");
        User user = userService.CreateAsync("admin@oidc.core", username, password).Result;
        Console.WriteLine($"Initial user created successfully, GUID: {user.Id}");

        return user;
    }

    private static async Task AssignRoles(AppDbContext context, User user)
    {
        Console.WriteLine("Assigning all available roles to initial user...");
        
        List<Role> roles = await context.Roles.ToListAsync();
        roles.ForEach(r =>
        {
            user.Roles.Add(new UserRole
            {
                Role = r,
                RoleId = r.Id,
                User = user,
                UserId = user.Id
            });
        });

        context.Users.Update(user);
        await context.SaveChangesAsync();
        
        Console.WriteLine("Roles assigned");
    }
}

