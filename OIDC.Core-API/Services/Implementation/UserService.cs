
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Users;
using OAuthServer.Exceptions;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IRoleService _roleService;

        public UserService(AppDbContext context, IRoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        public async Task<User> FindAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserApplications)
                .Include(u => u.AccessTokens)
                .Include(u => u.Applications)
                .FirstOrDefaultAsync(u => u.Id.Equals(id));
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public bool VerifyPassword(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<User> CreateAsync(string email, string username, string password)
        {
            User duplicate = await FindByEmailAsync(email);
            if (duplicate != null)
            {
                throw new DuplicateUserException($"An account is already registered with email: {email}");
            }
            
            User user = new User
            {
                Email = email,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = new List<UserRole>(),
                CreatedAt = DateTime.UtcNow
            };
            
            Role userRole = await _roleService.FindByNameAsync("User");
            user.Roles.Add(new UserRole
            {
                User = user,
                Role = userRole
            });
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User user, UserUpdateViewModel vm)
        {
            user.Username = vm.Username ?? user.Username;
            user.Email = vm.Email ?? user.Email;
            user.Bio = vm.Bio ?? user.Bio;
            user.Forename = vm.Forename ?? user.Forename;
            user.Surname = vm.Surname ?? user.Surname;
            user.DateOfBirth = vm.DateOfBirth ?? user.DateOfBirth;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}