using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IUserApplicationService
    {
        Task<UserApplication> FindOrCreateByUserAndClientIdAsync(User user, Guid clientId);
        Task<UserApplication> FindByUserAndApplicationAsync(User user, Application application);
        Task<UserApplication> AuthoriseApplicationAsync(Guid userId, Guid applicationId);
        Task<UserApplication> AuthoriseApplicationAsync(User user, Application application);
    }
}