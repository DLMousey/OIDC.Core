using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Users;

namespace OAuthServer.Services.Interface
{
    public interface IUserService
    {
        Task<User> FindAsync(Guid id);
        
        Task<User> FindByEmailAsync(string email);

        Task<User> CreateAsync(string email, string username, string password);

        Task<User> UpdateAsync(User user, UserUpdateViewModel vm);

        bool VerifyPassword(string hash, string password);
    }
}