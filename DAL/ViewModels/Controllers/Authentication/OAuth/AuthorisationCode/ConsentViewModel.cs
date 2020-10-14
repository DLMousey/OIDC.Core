using System;
using System.Collections.Generic;
using OAuthServer.DAL.Entities;

namespace OAuthServer.DAL.ViewModels.Controllers.Authentication.OAuth.AuthorisationCode
{
    public class ConsentViewModel
    {
        public Guid ApplicationId { get; set; }
    }
}