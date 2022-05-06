using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Role>> FindAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> FindByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name.Equals(name));
        }
    }
}