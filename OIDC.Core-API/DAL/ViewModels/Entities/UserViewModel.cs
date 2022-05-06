using System;

namespace OAuthServer.DAL.ViewModels.Entities
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}