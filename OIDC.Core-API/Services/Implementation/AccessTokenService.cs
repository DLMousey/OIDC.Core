using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly AppDbContext _context;
        private readonly IRandomValueService _randomValueService;
        private readonly IJwtService _jwtService;

        public AccessTokenService(AppDbContext context, IRandomValueService randomValueService, IJwtService jwtService)
        {
            _context = context;
            _randomValueService = randomValueService;
            _jwtService = jwtService;
        }

        public async Task<AccessToken> FindByCodeAsync(string code)
        {
            return await _context.AccessTokens
                .Include(at => at.User)
                .Include(at => at.Application)
                .FirstOrDefaultAsync(at => at.Code.Equals(code));
        }

        public async Task<AccessToken> FindByUserApplication(UserApplication userApplication)
        {
            return await _context.AccessTokens.FirstOrDefaultAsync(at =>
                at.UserId.Equals(userApplication.UserId) 
                && at.ApplicationId.Equals(userApplication.ApplicationId)
                && at.ExpiresAt > DateTime.UtcNow
            );
        }
        
        public async Task<AccessToken> CreateAsync(User user, Application application)
        {
            UserApplication assignment = new UserApplication
            {
                Application = application,
                ApplicationId = application.Id,
                User = user,
                UserId = user.Id
            };

            AccessToken accessToken = await FindByUserApplication(assignment);
            if (accessToken != null)
            {
                accessToken.LastUsed = DateTime.UtcNow;
                // _context.Update(accessToken);
                // await _context.SaveChangesAsync();
                
                return accessToken;
            }
            
            accessToken = new AccessToken
            {
                ApplicationId = application.Id,
                Application = application,
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            
            accessToken.Code = _jwtService.CreateJwt(accessToken);
            
            // await _context.AddAsync(accessToken);
            // await _context.SaveChangesAsync();
            return accessToken;
        }
    }
}