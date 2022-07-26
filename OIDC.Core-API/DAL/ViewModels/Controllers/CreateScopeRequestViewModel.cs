using System.ComponentModel.DataAnnotations;

namespace OAuthServer.DAL.ViewModels.Controllers;

public class CreateScopeRequestViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Label { get; set; }
    
    public string Description { get; set; }

    public bool Dangerous { get; set; } = true;

    public string Icon { get; set; }
}