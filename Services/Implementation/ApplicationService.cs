using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _context;

        public ApplicationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application> FindAsync(Guid id)
        {
            return await _context.Applications.FindAsync(id);
        }

        public async Task<Application> FindByClientIdAsync(Guid clientId)
        {
            return await _context.Applications.FirstOrDefaultAsync(a => a.ClientId.Equals(clientId));
        }
    }
}