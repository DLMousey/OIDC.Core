using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.ViewModels.Entities;

namespace OAuthServer.DAL.Entities
{
    public class Application
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid AuthorId { get; set; }
        
        public User Author { get; set; }

        public string HomepageUrl { get; set; }

        public string RedirectUrl { get; set; }

        public Guid ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool FirstParty { get; set; }

        public ICollection<UserApplication> UserApplications { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ApplicationViewModel ToViewModel()
        {
            return new ApplicationViewModel
            {
                Id = Id,
                Name = Name,
                Description = Description,
                FirstParty = FirstParty,
                HomepageUrl = HomepageUrl,
                RedirectUrl = RedirectUrl
            };
        }
    }
}