using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers;
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

        public async Task<Scope> FindByIdAsync(Guid id)
        {
            return await _context.Scopes.FindAsync(id);
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

        public async Task<Scope> CreateAsync(CreateScopeRequestViewModel request)
        {
            IList<Scope> duplicateScopes = await FindByNameAsync(new[] {request.Name});

            if (duplicateScopes.Count > 0)
            {
                throw new ArgumentException($"A scope already exists with the name '{request.Name}'");
            }
            
            Scope scope = new Scope
            {
                Name = request.Name,
                Description = request.Description,
                Label = request.Label,
                Icon = request.Icon,
                Dangerous = request.Dangerous
            };

            await _context.Scopes.AddAsync(scope);
            await _context.SaveChangesAsync();

            return scope;
        }

        public async Task<List<Scope>> CreateAsync(IEnumerable<CreateScopeRequestViewModel> request)
        {
            List<Scope> scopes = new();
            List<Scope> duplicateScopes = (List<Scope>) await FindByNameAsync(
                request.Select(s => s.Name).ToArray()
            );

            List<CreateScopeRequestViewModel> req = request.ToList();

            for (int i = 0; i < duplicateScopes.Count; i++)
            {
                req.RemoveAll(r => r.Name.Equals(duplicateScopes[i].Name));
            }

            foreach (CreateScopeRequestViewModel createRequest in req)
            {
                Scope scope = new Scope
                {
                    Name = createRequest.Name,
                    Description = createRequest.Description,
                    Label = createRequest.Label,
                    Icon = createRequest.Icon,
                    Dangerous = createRequest.Dangerous
                };

                await _context.Scopes.AddAsync(scope);
                scopes.Add(scope);
            }

            await _context.SaveChangesAsync();
            return scopes;
        }

        public async Task<Scope> UpdateAsync(Scope scope, CreateScopeRequestViewModel request)
        {
            scope.Name = request.Name;
            scope.Description = request.Description;
            scope.Label = request.Label;
            scope.Dangerous = request.Dangerous;
            scope.Icon = request.Icon;

            _context.Scopes.Update(scope);
            await _context.SaveChangesAsync();

            return scope;
        }
    }
}