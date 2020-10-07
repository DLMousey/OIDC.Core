using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers.Authentication
{
    [ApiController]
    [Route("oauth/verify")]
    public class TokenCheckController : ControllerBase
    {
        [Authorise]
        [HttpGet]
        public async Task<IActionResult> AccessCheck()
        {
            return Ok(new
            {
                status = 200,
                message = "Access token verified!",
                data = HttpContext.Items["User"]
            });
        } 
    }
}