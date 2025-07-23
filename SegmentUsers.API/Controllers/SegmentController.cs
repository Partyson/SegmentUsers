using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;

namespace SegmentUsers.API.Controllers;

[ApiController]
[Authorize]
[Route("api/segments")]
public class SegmentController(ISegmentService segmentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentDto createSegmentDto)
    {
        var createdSegmentId = await segmentService.CreateSegment(createSegmentDto);
        if (createdSegmentId == Guid.Empty)
            return BadRequest("Invalid segment");
        return Ok(createdSegmentId);
    }

    [HttpPost("{segmentId:guid}")]
    public async Task<IActionResult> AddSegmentForAnyPercentRandomVkUsers([FromRoute] Guid segmentId, [FromBody] int percent)
    {
        var result = await segmentService.AddSegmentForAnyPercentRandomVkUsers(segmentId, percent);
        
        return result ? Ok(result) : BadRequest();
    }

    [HttpPost("users/{segmentId:guid}")]
    public async Task<IActionResult> AddSomeVkUsersForSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.AddSomeUsersForSegment(segmentId, userIds);
        return result ? Ok(result) : BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSegments()
    {
        var segments = await segmentService.GetSegments();
        return Ok(segments);
    }

    [HttpGet("{segmentId:guid}")]
    public async Task<IActionResult> GetSegment([FromRoute] Guid segmentId)
    {
        var segment = await segmentService.GetSegment(segmentId);
        return segment is not null ? Ok(segment) : NotFound();
    }
    
    [HttpPatch("{segmentId:guid}")]
    public async Task<IActionResult> UpdateSegment([FromRoute] Guid segmentId, [FromBody] UpdateSegmentDto updateSegmentDto)
    {
        var result = await segmentService.UpdateSegment(segmentId, updateSegmentDto);
        return result ? Ok(result) : BadRequest();
    }
    
    [HttpDelete("users/{segmentId:guid}")]
    public async Task<IActionResult> DeleteSomeUsersFromSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.DeleteSomeUsersFromSegment(segmentId, userIds);
        return result ? Ok(result) : BadRequest();
    }
    
    [HttpDelete("{segmentId:guid}")]
    public async Task<IActionResult> DeleteSegment([FromRoute] Guid segmentId)
    {
        var result = await segmentService.DeleteSegment(segmentId);
        return result ? Ok(result) : BadRequest();
    }
}