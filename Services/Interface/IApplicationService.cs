using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> FindByClientIdAsync(Guid clientId);
    }
}