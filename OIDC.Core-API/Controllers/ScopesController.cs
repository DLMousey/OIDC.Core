using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Controllers;
using OAuthServer.Services.Interface;
using OAuthServer.Utility.Attributes;

namespace OAuthServer.Controllers;

[ApiController]
[Route("/scopes")]
public class ScopesController : ControllerBase
{
    private readonly IScopeService _scopeService;

    public ScopesController(IScopeService scopeService)
    {
        _scopeService = scopeService;
    }

    [Authorise("scopes.list")]
    [HttpGet]
    public async Task<IActionResult> ListAsync()
    {
        IList<Scope> scopes = await _scopeService.FindAllAsync();

        return new JsonResult(new
        {
            status = 200,
            message = "Scopes listed successfully",
            data = scopes
        }) { StatusCode = StatusCodes.Status200OK };
    }

    [Authorise("scopes.create")]
    [AuthoriseRoles("admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateScopeRequestViewModel request)
    {
        return NoContent();
    }

    [Authorise("scopes.create")]
    [HttpPost("batch")]
    public async Task<IActionResult> CreateMultipleAsync(IEnumerable<CreateScopeRequestViewModel> request)
    {
        return NoContent();
    }
}