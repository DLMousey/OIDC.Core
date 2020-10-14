using System;

namespace OAuthServer.DAL.ViewModels.Controllers.Users
{
    public class UserUpdateViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}