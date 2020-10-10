using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IAuthorisationCodeService
    {
        Task<AuthorisationCode> FindByCodeAsync(string authorisationCode);
        
        Task<AuthorisationCode> CreateAsync(User user, Application application);
    }
}