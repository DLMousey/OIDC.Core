using System;
using System.Collections.Generic;
using OAuthServer.DAL.ViewModels.Entities;

namespace OAuthServer.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Bio { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool Banned { get; set; } = false;

        public IList<UserApplication> UserApplications { get; set; }

        public IList<AccessToken> AccessTokens { get; set; }

        public IList<AuthorisationCode> AuthorisationCodes { get; set; }

        public IList<RefreshToken> RefreshTokens { get; set; }

        public IList<Application> Applications { get; set; }

        public IList<UserRole> Roles { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? BannedAt { get; set; }

        public UserViewModel ToViewModel()
        {
            return new UserViewModel
            {
                Id = Id,
                Username = Username,
                Email = Email,
                Bio = Bio,
                Forename = Forename,
                Surname = Surname,
                DateOfBirth = DateOfBirth
            };
        }
    }
}