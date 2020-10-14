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
        private readonly IUserApplicationService _userApplicationService;
        private readonly IScopeService _scopeService;
        private readonly IAuthorisationCodeService _authorisationCodeService;
        private readonly IAccessTokenService _accessTokenService;

        public AuthorisationCodeGrantController(
            IApplicationService applicationService, 
            IUserApplicationService userApplicationService,
            IScopeService scopeService,
            IAuthorisationCodeService authorisationCodeService,
            IAccessTokenService accessTokenService)
        {
            _applicationService = applicationService;
            _userApplicationService = userApplicationService;
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

            /*
             * "Authorise" the application at this point by creating a new user application record
             * along with the scopes requested by the application - this allows us to verify
             * that the same set of scopes that the user originally consented to are the same ones
             * the application ultimately ends up getting access to.
             *
             * No credentials will be generated at this point, ultimately leaving the user with
             * a linked application but no credentials that it can use - this can then be cleaned up
             * by a scheduled task later on which specifically looks for user application records
             * with no corresponding access tokens.
             */
            User user = (User) HttpContext.Items["User"];
            await _userApplicationService.AuthoriseApplicationAsync(user, application, scopes);

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
            
            /*
             * At this point we've verified the application ID being sent back is legitimate and that user is logged
             * in - indicating they have instructed us to generate an authorisation code for this application to
             * access their account.
             *
             * At this point we must validate this claim - if there is no user application record found,
             * the user has likely not been through the OAuth prompt and this request should be immediately
             * dropped since this request is malicious.
             */
            UserApplication userApplication =
                await _userApplicationService.FindByUserAndApplicationAsync(user, application);

            if (userApplication == null)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = "No user application link found - applications are not allowed to link to accounts " +
                              "without explicit user consent!"
                }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            
            AuthorisationCode authCode = await _authorisationCodeService.CreateAsync(user, application);

            /*
             * At this point in the process the user has consented to this application getting access
             * to their account and an authorisation token has been created, the user will be sent
             * back to the client with this authorisation token
             */
            await _userApplicationService.AuthoriseApplicationAsync(user, application);

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