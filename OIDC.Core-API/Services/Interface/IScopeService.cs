using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers;

namespace OAuthServer.Services.Interface
{
    public interface IScopeService
    {
        Task<IList<Scope>> FindAllAsync();
        
        Task<IList<Scope>> FindByNameAsync(string[] scopeNames);

        Task<Scope> CreateAsync(CreateScopeRequestViewModel request);

        Task<List<Scope>> CreateAsync(IEnumerable<CreateScopeRequestViewModel> request);
    }
}