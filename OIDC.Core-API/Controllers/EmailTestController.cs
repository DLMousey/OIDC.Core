using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers;

[ApiController]
[Route("/test/email")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [Authorise]
    [HttpGet]
    public async Task<IActionResult> TestEmailDispatch()
    {
        User user = (User) HttpContext.Items["User"];
        Dictionary<string, string> data = new();
        data.Add("username", user.Username);
        data.Add("login-time", DateTime.UtcNow.ToString("h:mm:ss tt zz"));
        
        await _emailService.SendToUserAsync("login.new", user, data);

        return Ok();
    }
}