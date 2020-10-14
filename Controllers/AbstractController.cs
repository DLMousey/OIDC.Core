using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Controllers
{
    public abstract class AbstractController : ControllerBase
    {
        public User GetUser()
        {
            return (User) HttpContext.Items["User"];
        }

        public Application GetApplication()
        {
            return (Application) HttpContext.Items["Application"];
        }
    }
}