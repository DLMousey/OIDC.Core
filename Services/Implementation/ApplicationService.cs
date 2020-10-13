using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;
using OAuthServer.Exceptions;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IRandomValueService _randomValueService;

        public ApplicationService(AppDbContext context, IRandomValueService randomValueService)
        {
            _context = context;
            _randomValueService = randomValueService;
        }

        public async Task<Application> FindAsync(Guid id)
        {
            return await _context.Applications.FindAsync(id);
        }

        public async Task<Application> FindByClientIdAsync(Guid clientId)
        {
            return await _context.Applications.FirstOrDefaultAsync(a => a.ClientId.Equals(clientId));
        }

        public async Task<Application> CreateAsync(CreateRequestViewModel vm, User user)
        {
            Application application = new Application
            {
                Name = vm.Name,
                Description = vm.Description,
                AuthorId = user.Id,
                Author = user,
                HomepageUrl = vm.HomepageUrl,
                RedirectUrl = vm.RedirectUrl,
                ClientId = Guid.NewGuid(),
                ClientSecret = _randomValueService.CryptoSafeRandomString(),
                FirstParty = false
            };

            await _context.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Application> UpdateAsync(CreateRequestViewModel vm, Guid id)
        {
            Application application = await FindAsync(id);

            if (application == null)
            {
                throw new UnknownApplicationException($"No application found by id: {id}");
            }

            application.Name = vm.Name;
            application.Description = vm.Description;
            application.HomepageUrl = vm.HomepageUrl;
            application.RedirectUrl = vm.RedirectUrl;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();

            return application;
        }
    }
}