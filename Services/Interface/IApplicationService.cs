using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> FindAsync(Guid id);
        Task<Application> FindByClientIdAsync(Guid clientId);
    }
}