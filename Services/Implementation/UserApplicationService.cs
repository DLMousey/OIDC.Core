using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Exceptions;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class UserApplicationService : IUserApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IApplicationService _applicationService;

        public UserApplicationService(AppDbContext context, IApplicationService applicationService)
        {
            _context = context;
            _applicationService = applicationService;
        }

        public async Task<UserApplication> FindOrCreateByUserAndClientIdAsync(User user, Guid clientId)
        {
            UserApplication userApplication = await _context.UserApplications
                .Include(ua => ua.User)
                .Include(ua => ua.Application)
                .FirstOrDefaultAsync(ua =>
                    ua.User.Equals(user) && ua.Application.ClientId.Equals(clientId)
                );

            if (userApplication != null)
            {
                return userApplication;
            }

            Application application = await _applicationService.FindByClientIdAsync(clientId);
            if (application == null)
            {
                throw new UnknownApplicationException($"Application with client id {clientId} not found");
            }

            userApplication = new UserApplication
            {
                Application = application,
                ApplicationId = application.Id,
                User = user,
                UserId = user.Id
            };

            await _context.AddAsync(userApplication);
            await _context.SaveChangesAsync();

            return userApplication;
        }

        public async Task<UserApplication> FindByUserAndApplicationAsync(User user, Application application)
        {
            return await _context.UserApplications
                .Include(ua => ua.Application)
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua =>
                ua.UserId.Equals(user.Id) && ua.ApplicationId.Equals(application.Id));
        }

        public async Task<UserApplication> AuthoriseApplicationAsync(Guid userId, Guid applicationId)
        {
            UserApplication userApplication = new UserApplication
            {
                ApplicationId = applicationId,
                UserId = userId
            };

            await _context.AddAsync(userApplication);
            await _context.SaveChangesAsync();

            return userApplication;
        }

        public async Task<UserApplication> AuthoriseApplicationAsync(User user, Application application)
        {
            UserApplication userApplication = new UserApplication
            {
                Application = application,
                ApplicationId = application.Id,
                User = user,
                UserId = user.Id
            };

            await _context.AddAsync(userApplication);
            await _context.SaveChangesAsync();

            return userApplication;
        }
    }
}