using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;

namespace OAuthServer.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> FindAsync(Guid id);
        Task<Application> FindByClientIdAsync(Guid clientId);
        Task<IList<Application>> FindByAuthorAsync(User user);
        Task<Application> CreateAsync(CreateRequestViewModel vm, User user);
        Task<Application> UpdateAsync(CreateRequestViewModel vm, Application application);
        Task<Application> DeleteAsync(Application application);
    }
}