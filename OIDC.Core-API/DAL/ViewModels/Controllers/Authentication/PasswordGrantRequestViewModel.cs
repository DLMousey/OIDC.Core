using System;
using System.ComponentModel.DataAnnotations;

namespace OAuthServer.DAL.ViewModels.Controllers.Authentication
{
    public class PasswordGrantRequestViewModel
    {
        [Required]
        public string GrantType { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}