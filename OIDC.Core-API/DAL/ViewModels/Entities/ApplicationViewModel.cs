using System;
using System.Collections.Generic;
using System.ComponentModel;
using OAuthServer.DAL.Entities;

namespace OAuthServer.DAL.ViewModels.Entities
{
    public class ApplicationViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string HomepageUrl { get; set; }

        public string RedirectUrl { get; set; }

        public bool FirstParty { get; set; }

        public Guid ClientId { get; set; }

        [Description("If this viewmodel is being returned in the context of the application's link to the user, " +
                     "a list of scopes the application has been granted by the user should be populated")]
        public List<Scope> Scopes { get; set; } = null;
    }
}