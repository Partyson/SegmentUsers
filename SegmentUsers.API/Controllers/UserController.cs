using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;

namespace SegmentUsers.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController(IVkUserService vkUserService) : ControllerBase
{
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddToSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.AddToSegments(userId, segmentIds);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetVkUser([FromRoute] Guid userId)
    {
        var user = await vkUserService.GetVkUser(userId);
        return user is not null ? Ok(user): NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVkUsers()
    {
        var users = await vkUserService.GetVkUsers();
        return Ok(users);
    }
    
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteFromSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.DeleteFromSegments(userId, segmentIds);
        return Ok(result);
    }
}