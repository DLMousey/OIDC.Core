using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;
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
    }
}