using System.ComponentModel.DataAnnotations;

namespace OAuthServer.DAL.ViewModels.Controllers.Registration
{
    public class RegistrationRequestViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}