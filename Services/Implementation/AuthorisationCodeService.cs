using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class AuthorisationCodeService : IAuthorisationCodeService
    {
        private readonly AppDbContext _context;

        public AuthorisationCodeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorisationCode> FindByCodeAsync(string authorisationCode)
        {
            return await _context.AuthorisationCodes
                .FirstOrDefaultAsync(ac => ac.Code.Equals(authorisationCode));
        }

        public async Task<AuthorisationCode> CreateAsync(User user, Application application)
        {
            AuthorisationCode code = new AuthorisationCode
            {
                Code = GenerateCode(),
                UserId = user.Id,
                User = user,
                ApplicationId = application.Id,
                Application = application
            };

            await _context.AddAsync(code);
            await _context.SaveChangesAsync();

            return code;
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