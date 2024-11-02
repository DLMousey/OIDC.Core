using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OAuthServer.DAL.ViewModels.Controllers.Authentication
{
    public class PasswordGrantRequestViewModel
    {
        [Required]
        [FromForm(Name = "grant_type")]
        public string GrantType { get; set; }

        [Required]
        [FromForm(Name = "username")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required] 
        [FromForm(Name = "scope")]
        public string ScopeNames { get; set; }
        
        public string[] Scopes { get; set; }
    }
}