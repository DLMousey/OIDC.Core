using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL.Entities;

namespace OAuthServer.DAL
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<AccessToken> AccessTokens { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<AuthorisationCode> AuthorisationCodes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Scope> Scopes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserApplication> UserApplications { get; set; }
        public virtual DbSet<UserApplicationScope> UserApplicationScopes { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccessToken>(builder =>
            {
                builder.HasOne(at => at.Application);
                builder.HasOne(at => at.User)
                    .WithMany(u => u.AccessTokens)
                    .HasForeignKey(at => at.UserId);
            });

            modelBuilder.Entity<Application>(builder =>
            {
                builder.HasOne(a => a.Author)
                    .WithMany(u => u.Applications)
                    .HasForeignKey(a => a.AuthorId);

                builder.HasMany(a => a.UserApplications)
                    .WithOne(ua => ua.Application)
                    .HasForeignKey(au => au.ApplicationId);
            });

            modelBuilder.Entity<AuthorisationCode>(builder =>
            {
                builder.HasOne(ac => ac.User)
                    .WithMany(u => u.AuthorisationCodes)
                    .HasForeignKey(ac => ac.UserId);

                // Link between authorisation code and application?
                builder.HasOne(ac => ac.Application);
            });

            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);

                builder.HasOne(rt => rt.Application);
            });
            
            modelBuilder.Entity<UserRole>(builder =>
            {
                builder.HasKey(ur => new {ur.UserId, ur.RoleId});
                
                builder.HasOne(ur => ur.User)
                    .WithMany(ur => ur.Roles)
                    .HasForeignKey(ur => ur.UserId);
                
                builder.HasOne(ur => ur.Role)
                    .WithMany(ur => ur.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            modelBuilder.Entity<UserApplication>(builder =>
            {
                builder.HasOne(ua => ua.User)
                    .WithMany(u => u.UserApplications)
                    .HasForeignKey(ua => ua.UserId);

                builder.HasMany(ua => ua.Scopes)
                    .WithOne(uas => uas.UserApplication)
                    .HasForeignKey(uas => uas.UserApplicationId);

                builder.HasOne(ua => ua.Application);
            });

            modelBuilder.Entity<UserApplicationScope>(builder =>
            {
                builder.HasKey(uas => new {uas.UserApplicationId, uas.ScopeId});

                builder.HasOne(uas => uas.UserApplication)
                    .WithMany(ua => ua.Scopes)
                    .HasForeignKey(ua => ua.UserApplicationId);

                builder.HasOne(uas => uas.Scope);
            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}