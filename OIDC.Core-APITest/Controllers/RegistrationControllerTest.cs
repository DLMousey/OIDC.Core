using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OAuthServer.Controllers.Registration;
using OAuthServer.DAL;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers.Registration;
using OAuthServer.Exceptions;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;
using Xunit;

namespace OIDC.Core_APITest.Controllers;

public class RegistrationControllerTest
{
    private readonly Mock<AppDbContext> _dbContext;
    private readonly Mock<IRoleService> _roleService;
    private readonly Mock<IUserService> _userService;
    private readonly RegistrationController _controller;

    public RegistrationControllerTest()
    {
        _dbContext = DbContextMockBuilder.BuildDbContextMock();

        _roleService = new Mock<IRoleService>();
        _userService = new Mock<IUserService>();
        _controller = new RegistrationController(_userService.Object);
    }

    [Fact]
    public async Task ReturnsErrorIfNoEmailProvided()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("email", "Required");

        IActionResult result = await _controller.Register(null);
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }

    [Fact]
    public async Task ReturnsErrorIfInvalidEmailProvided()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("email", "EmailAddress");
        
        IActionResult result = await _controller.Register(null);
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }

    [Fact]
    public async Task ReturnsErrorIfNoUsernameProvided()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("username", "Required");
        
        IActionResult result = await _controller.Register(null);
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }
    
    [Fact]
    public async Task ReturnsErrorIfNoPasswordProvided()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("passowrd", "Required");
        
        IActionResult result = await _controller.Register(null);
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }
    
    [Fact]
    public async Task ReturnsErrorIfShortPasswordProvided()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("username", "MinLength");
        
        IActionResult result = await _controller.Register(null);
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }

    [Fact]
    public async Task ReturnsErrorIfEmailIsRegistered()
    {
        _userService.Invocations.Clear();
        _userService.Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )
        ).ThrowsAsync(new DuplicateUserException("Test Exception Message"));
        

        RegistrationController controller = new RegistrationController(_userService.Object);
        IActionResult result = await controller.Register(vm: new RegistrationRequestViewModel
        {
            Email = "info@example.com",
            Username = "info-example-com",
            Password = "infoexamplecom"
        });
        
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }

    [Fact]
    public async Task RegistersUserIfInputOk()
    {
        _userService.Invocations.Clear();
        
        RegistrationController controller = new RegistrationController(_userService.Object);
        IActionResult result = await controller.Register(vm: new RegistrationRequestViewModel
        {
            Email = "info@example.com",
            Username = "info-example-com",
            Password = "infoexamplecom"
        });
        
        JsonResult actual = (result as JsonResult)!;
        
        Assert.NotNull(actual);
        Assert.Equal(201, actual.StatusCode);
    }
}