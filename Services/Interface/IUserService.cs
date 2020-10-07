using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IUserService
    {
        Task<User> FindAsync(Guid id);
        
        Task<User> FindByEmailAsync(string email);

        Task<User> CreateAsync(string email, string username, string password);

        bool VerifyPassword(string hash, string password);
    }
}