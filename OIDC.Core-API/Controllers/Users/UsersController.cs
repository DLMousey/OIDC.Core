using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Users;
using OAuthServer.DAL.ViewModels.Entities;
using OAuthServer.Services.Interface;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers.Users
{
    [ApiController]
    [Route("users")]
    public class UsersController : AbstractController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [Authorise]
        [HttpGet]
        [AuthoriseScopes("profile.read")]
        public async Task<IActionResult> Get()
        {
            User user = await _userService.FindAsync(GetUser().Id);
            Application application = GetApplication();

            UserViewModel userVm = user.ToViewModel();
            ApplicationViewModel applicationVm = application.ToViewModel();
            
            return Ok(new
            {
                status = 200,
                message = "User details retrieved successfully",
                data = new
                {
                    user = userVm,
                    application = applicationVm
                }
            });
        }

        [Authorise]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateViewModel vm)
        {
            User user = (User) HttpContext.Items["User"];
            user = await _userService.UpdateAsync(user, vm);

            return Ok(new
            {
                status = 200,
                message = "User updated successfully",
                data = user.ToViewModel()
            });
        }
    }
}