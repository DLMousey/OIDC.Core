using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IScopeService
    {
        Task<IList<Scope>> FindAllAsync();
        
        Task<IList<Scope>> FindByNameAsync(string[] scopeNames);
    }
}