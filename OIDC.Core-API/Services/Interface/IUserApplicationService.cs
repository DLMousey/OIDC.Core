using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IUserApplicationService
    {
        Task<IList<UserApplication>> FindByUserAsync(User user);
        Task<UserApplication> FindOrCreateByUserAndClientIdAsync(User user, Guid clientId);
        Task<UserApplication> FindByUserAndApplicationAsync(User user, Application application);
        Task<UserApplication> AuthoriseApplicationAsync(Guid userId, Guid applicationId);
        Task<UserApplication> AuthoriseApplicationAsync(User user, Application application);
        Task<UserApplication> AuthoriseApplicationAsync(User user, Application application, IList<Scope> scopes);
    }
}