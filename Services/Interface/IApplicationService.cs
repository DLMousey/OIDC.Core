using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;

namespace OAuthServer.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> FindAsync(Guid id);
        Task<Application> FindByClientIdAsync(Guid clientId);
        Task<Application> CreateAsync(CreateRequestViewModel vm, User user);
    }
}