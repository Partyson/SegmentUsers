using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SegmentUsers.Application.Interfaces;

namespace SegmentUsers.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddToSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await userService.AddToSegments(userId, segmentIds);
        return Ok(result);
    }

    [HttpGet("segments/{userId:guid}")]
    public async Task<IActionResult> GetUserSegments([FromRoute] Guid userId)
    {
        var userSegments = await userService.GetUserSegments(userId);
        return Ok(userSegments);
    }
    
    [HttpGet("segments")]
    public async Task<IActionResult> GetAllUserSegments()
    {
        var userSegments = await userService.GetAllUserSegments();
        return Ok(userSegments);
    }
    
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteFromSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await userService.DeleteFromSegments(userId, segmentIds);
        return Ok(result);
    }
}