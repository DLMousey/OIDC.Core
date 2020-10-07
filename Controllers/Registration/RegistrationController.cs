using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Registration;
using OAuthServer.Exceptions;
using OAuthServer.Services.Interface;

namespace OAuthServer.Controllers.Registration
{
    [ApiController]
    [Route("registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegistrationController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestViewModel vm)
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

            User user;
            try
            {
                user = await _userService.CreateAsync(vm.Email, vm.Username, vm.Password);
            }
            catch (DuplicateUserException ex)
            {
                return new JsonResult(new
                {
                    status = 400,
                    message = ex.Message
                }) { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new JsonResult(new
            {
                status = 200,
                message = "Registration successful",
                data = user
            });
        }
    }
}