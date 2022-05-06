using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Application> UpdateAsync(CreateRequestViewModel vm, Application application)
        {
            application.Name = vm.Name;
            application.Description = vm.Description;
            application.HomepageUrl = vm.HomepageUrl;
            application.RedirectUrl = vm.RedirectUrl;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Application> DeleteAsync(Application application)
        {
            List<UserApplication> userApplications = await _context.UserApplications
                .Include(ua => ua.Application)
                .Where(ua => ua.Application.Equals(application))
                .ToListAsync();

            List<AccessToken> accessTokens = await _context.AccessTokens
                .Include(at => at.Application)
                .Where(at => at.Application.Equals(application))
                .ToListAsync();

            List<AuthorisationCode> authorisationCodes = await _context.AuthorisationCodes
                .Include(ac => ac.Application)
                .Where(ac => ac.Application.Equals(application))
                .ToListAsync();
            
            _context.RemoveRange(userApplications);
            _context.RemoveRange(accessTokens);
            _context.RemoveRange(authorisationCodes);
            _context.Applications.Remove(application);

            await _context.SaveChangesAsync();

            return null;
        }
    }
}