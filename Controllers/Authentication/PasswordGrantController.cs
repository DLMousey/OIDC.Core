using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Authentication;
using OAuthServer.Exceptions;
using OAuthServer.Services.Interface;

namespace OAuthServer.Controllers.Authentication
{
    [ApiController]
    [Route("oauth/password")]
    public class PasswordGrantController : ControllerBase
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IUserApplicationService _userApplicationService;
        private readonly IUserService _userService;

        public PasswordGrantController(IAccessTokenService accessTokenService, 
            IUserApplicationService userApplicationService, IUserService userService)
        {
            _accessTokenService = accessTokenService;
            _userApplicationService = userApplicationService;
            _userService = userService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] PasswordGrantRequestViewModel vm)
        {
            if (vm.GrantType.ToLower() != "password" || !ModelState.IsValid)
            {
                string message = (ModelState.IsValid)
                    ? "Unknown grant type provided - Only password grant types are available at this endpoint"
                    : "Validation failed";
                
                return new JsonResult(new
                {
                    status = 400, 
                    message,
                    data = (!ModelState.IsValid) ? ModelState : null
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            User user = await _userService.FindByEmailAsync(vm.Email);
            if (user == null || !_userService.VerifyPassword(user.Password, vm.Password))
            {
                return new JsonResult(new
                {
                    status = 401,
                    message = "Invalid credentials provided"
                }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            UserApplication userApplication;
            try
            {
                userApplication = await _userApplicationService.FindOrCreateByUserAndClientIdAsync(user, vm.ClientId);
            }
            catch (UnknownApplicationException ex)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = ex.Message
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            if (!userApplication.Application.FirstParty)
            {
                return new JsonResult(new
                {
                    status = 403,
                    message = "Third party applications are not permitted to use password grants"
                }) {StatusCode = StatusCodes.Status403Forbidden};
            }
            
            AccessToken accessToken = await _accessTokenService.CreateAsync(user, userApplication.Application);
            
            CookieOptions options = new CookieOptions();
            options.Expires = accessToken.ExpiresAt;
            options.HttpOnly = true;
            Response.Cookies.Append("_oidc.core-token", accessToken.Code, options);
            
            return Ok(new
            {
                status = 200,
                message = "Authentication successful",
                data = accessToken.Code
            });
        }
    }
}