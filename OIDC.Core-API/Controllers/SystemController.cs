using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OAuthServer.Controllers;

[ApiController]
[Route("/system")]
public class SystemController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> HealthCheckAsync()
    {
        return NoContent();
    }
}