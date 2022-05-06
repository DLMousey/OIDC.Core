using System;
using System.ComponentModel.DataAnnotations;

namespace OAuthServer.DAL.ViewModels.Controllers.Authentication.OAuth.AuthorisationCode
{
    public class PromptRequestViewModel
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string State { get; set; }
        
        public string RedirectUri { get; set; }

        [Required]
        public string Scopes { get; set; }
    }
}