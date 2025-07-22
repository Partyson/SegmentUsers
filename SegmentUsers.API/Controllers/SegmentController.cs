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
        return Ok(createdSegmentId);
    }

    [HttpPost("{segmentId:guid}")]
    public async Task<IActionResult> AddSegmentForAnyPercentRandomUsers([FromRoute] Guid segmentId, [FromBody] int percent)
    {
        var result = await segmentService.AddSegmentForAnyPercentRandomUsers(segmentId, percent);
        return Ok(result);
    }

    [HttpPost("users/{segmentId:guid}")]
    public async Task<IActionResult> AddSomeUsersForSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.AddSomeUsersForSegment(segmentId, userIds);
        return Ok(result);
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
        return Ok(segment);
    }
    
    [HttpPatch("{segmentId:guid}")]
    public async Task<IActionResult> UpdateSegment([FromRoute] Guid segmentId, [FromBody] UpdateSegmentDto updateSegmentDto)
    {
        var result = await segmentService.UpdateSegment(segmentId, updateSegmentDto);
        return Ok(result);
    }
    
    [HttpDelete("users/{segmentId:guid}")]
    public async Task<IActionResult> DeleteSomeUsersFromSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.DeleteSomeUsersFromSegment(segmentId, userIds);
        return Ok(result);
    }
    
    [HttpDelete("{segmentId:guid}")]
    public async Task<IActionResult> DeleteSegment([FromRoute] Guid segmentId)
    {
        var result = await segmentService.DeleteSegment(segmentId);
        return Ok(result);
    }
}