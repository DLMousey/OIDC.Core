using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IAccessTokenService
    {
        Task<AccessToken> FindByCodeAsync(string code);
        Task<AccessToken> FindByUserApplication(UserApplication userApplication);
        Task<AccessToken> CreateAsync(User user, Application application);
    }
}