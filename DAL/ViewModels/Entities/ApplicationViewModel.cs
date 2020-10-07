using System;

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
    }
}