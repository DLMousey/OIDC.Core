using System;
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
        if (!ModelState.IsValid)
        {
            return new JsonResult(new
            {
                status = 400,
                message = "Validation failed, please double check your data and try again",
                data = ModelState
            }) { StatusCode = StatusCodes.Status400BadRequest };
        }

        try
        {
            Scope scope = await _scopeService.CreateAsync(request);
            return new JsonResult(new
            {
                status = 201,
                message = "Scope created successfully",
                data = scope
            });
        }
        catch (ArgumentException ex)
        {
            return new JsonResult(new
            {
                status = 400,
                message = ex.Message,
                data = request
            }) { StatusCode = StatusCodes.Status400BadRequest };
        }

    }

    [Authorise("scopes.create")]
    [AuthoriseRoles("admin")]
    [HttpPost("batch")]
    public async Task<IActionResult> CreateMultipleAsync(IEnumerable<CreateScopeRequestViewModel> request)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new
            {
                status = 400,
                message = "Validation failed, please double check your data and try again",
                data = ModelState
            }) { StatusCode = StatusCodes.Status400BadRequest };
        }

        List<Scope> scopes = await _scopeService.CreateAsync(request);
        return new JsonResult(new
        {
            status = 201,
            message = "Scopes created successfully",
            data = scopes
        }) { StatusCode = StatusCodes.Status201Created };
    }

    [Authorise("scopes.update")]
    [AuthoriseRoles("admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateScopeRequestViewModel request)
    {
        Scope scope = await _scopeService.FindByIdAsync(id);

        if (scope == null)
        {
            return new JsonResult(new
            {
                status = 400,
                message = "Invalid scope ID provided"
            }) { StatusCode = StatusCodes.Status400BadRequest };
        }

        if (!ModelState.IsValid)
        {
            return new JsonResult(new
            {
                status = 400,
                message = "Validation failed, please double check your data and try again",
                data = ModelState
            }) { StatusCode = StatusCodes.Status400BadRequest };
        }

        scope = await _scopeService.UpdateAsync(scope, request);

        return new JsonResult(new
        {
            status = 200,
            message = "Scope updated successfully",
            data = scope
        }) { StatusCode = StatusCodes.Status200OK };
    }
}