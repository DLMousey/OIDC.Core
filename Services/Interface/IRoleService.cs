using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface
{
    public interface IRoleService
    {
        public Task<IList<Role>> FindAll();

        public Task<Role> FindByNameAsync(string name);
    }
}