using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                .Select(ua =>
                {
                    List<Scope> scopes = ua.Scopes.Select(ua => ua.Scope).ToList();
                    ApplicationViewModel vm = ua.Application.ToViewModel();
                    vm.Scopes = scopes;

                    return vm;
                })
                .ToList();

            return Ok(new
            {
                status = 200,
                message = "Authorised applications retrieved successfully",
                data = applications
            });
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedApplications()
        {
            User user = (User) HttpContext.Items["User"];

            IList<Application> applications = await _applicationService.FindByAuthorAsync(user);
            IList<ApplicationViewModel> applicationVms = applications
                .Select(a => a.ToViewModel())
                .ToList();

            return Ok(new
            {
                status = 200,
                message = "Published applications retrieved successfully",
                data = applicationVms
            });
        }

        [Authorise]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed",
                    data = ModelState
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            User user = (User) HttpContext.Items["User"];
            Application application = await _applicationService.CreateAsync(vm, user);
            return new JsonResult(new
            {
                status = 201,
                message = "Application created successfully",
                data = new
                {
                    application = new
                    {
                        id = application.Id,
                        name = application.Name,
                        description = application.Description,
                        homepageUrl = application.HomepageUrl,
                        redirectUrl = application.RedirectUrl,
                        clientId = application.ClientId,
                        clientSecret = application.ClientSecret,
                        createdAt = application.CreatedAt
                    }
                }
            });
        }

        [Authorise]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] CreateRequestViewModel vm, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed",
                    data = ModelState
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            User user = (User) HttpContext.Items["User"];
            Application application = await _applicationService.FindAsync(id);

            if (application == null || application.AuthorId != user.Id)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Invalid application id provided",
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            

            application = await _applicationService.UpdateAsync(vm, application);
            ApplicationViewModel appVm = application.ToViewModel();

            return new JsonResult(new
            {
                status = 200,
                message = "Application updated successfully",
                data = new
                {
                    application = appVm
                }
            });
        }

        [Authorise]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            User user = (User) HttpContext.Items["User"];
            Application application = await _applicationService.FindAsync(id);

            if (application == null || application.AuthorId != user.Id)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Invalid application id provided"
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            await _applicationService.DeleteAsync(application);
            return NoContent();
        }
    }
} 