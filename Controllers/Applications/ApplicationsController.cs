using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Applications;
using OAuthServer.DAL.ViewModels.Entities;
using OAuthServer.Services.Interface;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers.Applications
{
    [Authorise]
    [ApiController]
    [Route("applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IUserApplicationService _userApplicationService;

        public ApplicationsController(IApplicationService applicationService, IUserApplicationService userApplicationService)
        {
            _applicationService = applicationService;
            _userApplicationService = userApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            User user = (User) HttpContext.Items["User"];

            IList<UserApplication> authorisedApplications = await _userApplicationService
                .FindByUserAsync(user);

            IList<ApplicationViewModel> applications = authorisedApplications
                .Select(ua => ua.Application.ToViewModel())
                .ToList();

            return Ok(new
            {
                status = 200,
                message = "Authorised applications retrieved successfully",
                data = applications
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationCreateRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed",
                    data = ModelState
                });
            }

            // @TODO - Implement endpoint
            return Ok();
        }
    }
}