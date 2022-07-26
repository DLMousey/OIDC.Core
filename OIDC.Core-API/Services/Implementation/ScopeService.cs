using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class ScopeService : IScopeService
    {
        private readonly AppDbContext _context;

        public ScopeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Scope>> FindAllAsync()
        {
            return await _context.Scopes.ToListAsync();
        }

        public async Task<IList<Scope>> FindByNameAsync(string[] scopeNames)
        {
            IList<Scope> scopes = new List<Scope>();

            foreach (string scopeName in scopeNames)
            {
                Scope scope = await _context.Scopes.FirstOrDefaultAsync(s => s.Name.Equals(scopeName));
                if (scope != null)
                {
                    scopes.Add(scope);
                }
            }

            return scopes;
        }
    }
}