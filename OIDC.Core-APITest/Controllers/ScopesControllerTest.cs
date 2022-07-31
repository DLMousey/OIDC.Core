using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OAuthServer.Controllers;
using OAuthServer.DAL;
using OAuthServer.DAL.ViewModels.Controllers;
using OAuthServer.Services.Interface;
using Org.BouncyCastle.Tls;
using Xunit;

namespace OIDC.Core_APITest.Controllers;

public class ScopesControllerTest
{
    private readonly Mock<AppDbContext> _dbContext;
    private readonly Mock<IScopeService> _scopeService;
    private readonly ScopesController _controller;
    
    public ScopesControllerTest()
    {
        _dbContext = DbContextMockBuilder.BuildDbContextMock();

        _scopeService = new Mock<IScopeService>();
        _controller = new ScopesController(_scopeService.Object);
    }

    [Fact]
    public async Task CreateSingleReturnsErrorIfModelStateInvalid()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("CatchAllField", "Required");

        IActionResult result = await _controller.CreateAsync(null);
        JsonResult actual = (result as JsonResult)!;

        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }

    [Fact]
    public async Task CreateMultipleReturnsErrorIfModelStateInvalid()
    {
        _controller.ModelState.Clear();
        _controller.ModelState.AddModelError("CatchAllField", "Required");

        IActionResult result = await _controller.CreateAsync(null);
        JsonResult actual = (result as JsonResult)!;

        Assert.NotNull(actual);
        Assert.Equal(400, actual.StatusCode);
    }
}