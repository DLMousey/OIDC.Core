using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly AppDbContext _context;
        private readonly IRandomValueService _randomValueService;

        public AccessTokenService(AppDbContext context, IRandomValueService randomValueService)
        {
            _context = context;
            _randomValueService = randomValueService;
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
                _context.Update(accessToken);
                await _context.SaveChangesAsync();
                
                return accessToken;
            }
            
            accessToken = new AccessToken
            {
                ApplicationId = application.Id,
                Code = GenerateCode(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            
            await _context.AddAsync(accessToken);
            await _context.SaveChangesAsync();
            return accessToken;
        }
        
        public async Task<AccessToken> CreateAsync(User user, UserApplication application)
        {
            AccessToken accessToken = await FindByUserApplication(application);
            if (accessToken != null)
            {
                accessToken.LastUsed = DateTime.UtcNow;
                _context.Update(accessToken);
                await _context.SaveChangesAsync();
                
                return accessToken;
            }
            
            accessToken = new AccessToken
            {
                ApplicationId = application.Id,
                Code = GenerateCode(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            
            await _context.AddAsync(accessToken);
            await _context.SaveChangesAsync();
            return accessToken;
        }

        private string GenerateCode()
        {
            int length = 64;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
    }
}