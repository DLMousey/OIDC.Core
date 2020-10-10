using System;

namespace OAuthServer.DAL.ViewModels.Controllers.Authentication.OAuth.AuthorisationCode
{
    public class TokenExchangeViewModel
    {
        public string AuthorisationCode { get; set; }

        public Guid ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}