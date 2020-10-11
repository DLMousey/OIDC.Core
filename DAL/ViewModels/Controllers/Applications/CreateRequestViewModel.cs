using System.ComponentModel.DataAnnotations;

namespace OAuthServer.DAL.ViewModels.Controllers.Applications
{
    public class CreateRequestViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Url]
        public string HomepageUrl { get; set; }

        [Required]
        [Url]
        public string RedirectUrl { get; set; }
    }
}