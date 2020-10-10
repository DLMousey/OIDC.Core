using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Authentication.OAuth.AuthorisationCode;
using OAuthServer.DAL.ViewModels.Entities;
using OAuthServer.Services.Interface;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers.Authentication
{
    [Route("oauth/authorisation-code")]
    [ApiController]
    public class AuthorisationCodeGrantController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IScopeService _scopeService;
        private readonly IAuthorisationCodeService _authorisationCodeService;
        private readonly IAccessTokenService _accessTokenService;

        public AuthorisationCodeGrantController(
            IApplicationService applicationService, 
            IScopeService scopeService,
            IAuthorisationCodeService authorisationCodeService,
            IAccessTokenService accessTokenService)
        {
            _applicationService = applicationService;
            _scopeService = scopeService;
            _authorisationCodeService = authorisationCodeService;
            _accessTokenService = accessTokenService;
        }

        [Authorise]
        [HttpGet]
        public async Task<IActionResult> GetPromptInfo([FromQuery] PromptRequestViewModel vm)
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
            
            string[] scopeNames = vm.Scopes.Split(",");
            IList<Scope> scopes = await _scopeService.FindByNameAsync(scopeNames);
            Application application = await _applicationService.FindByClientIdAsync(vm.ClientId);

            if (scopes.Count == 0 || application == null)
            {
                string message = (scopes.Count == 0) ? "At least 1 scope must be provided" 
                    : "Invalid client id provided";
                
                return new JsonResult(new
                {
                    status = 400,
                    message
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            
            // @TODO - Generate a cryptographically signed OTP that will be returned to this endpoint to prevent token generation without consent
            
            ApplicationViewModel applicationVm = application.ToViewModel();
            return new JsonResult(new
            {
                status = 200,
                message = "Authorisation code prompt info retrieved successfully",
                data = new
                {
                    scopes,
                    application = applicationVm
                }
            });
        }

        [Authorise]
        [HttpPost]
        public async Task<IActionResult> GenerateAuthorisationCode([FromBody] ConsentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed",
                    data = ModelState
                }) {StatusCode = StatusCodes.Status400BadRequest};
            }

            Application application = await _applicationService.FindAsync(vm.ApplicationId);
            if (application == null)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message =
                        "Invalid application id specified - please provide the application's GUID and not it's client id"
                }) {StatusCode = StatusCodes.Status400BadRequest};
            }

            User user = (User) HttpContext.Items["User"];
            AuthorisationCode authCode = await _authorisationCodeService.CreateAsync(user, application);

            return Ok(new
            {
                status = 200,
                message = "Authorisation code generated successfully",
                data = new
                {
                    authorisationCode = authCode.Code
                }
            });
        }

        [Authorise]
        [HttpPost("exchange")]
        public async Task<IActionResult> TokenExchange([FromBody] TokenExchangeViewModel vm)
        {
            Application application = await _applicationService.FindByClientIdAsync(vm.ClientId);

            if (application == null || vm.ClientSecret != application.ClientSecret)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed - double check your parameters and try again"
                }) { StatusCode = StatusCodes.Status400BadRequest }; 
            }

            AuthorisationCode authCode = await _authorisationCodeService.FindByCodeAsync(vm.AuthorisationCode);
            if (authCode == null)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "Validation failed - double check your parameters and try again"
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            User user = (User) HttpContext.Items["User"];
            AccessToken token = await _accessTokenService.CreateAsync(user, authCode.Application);

            return Ok(new
            {
                status = 200,
                message = "Token exchanged successfully",
                data = new
                {
                    code = token.Code,
                    type = "Bearer",
                    expires = token.ExpiresAt.ToString(CultureInfo.InvariantCulture)
                }
            });
        }
    }
}