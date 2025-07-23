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
    /// <summary>
    /// Создает новый сегмент.
    /// </summary>
    /// <param name="createSegmentDto">DTO с данными нового сегмента.</param>
    /// <returns>Id созданного сегмента или ошибка 400, если данные некорректны.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentDto createSegmentDto)
    {
        var createdSegmentId = await segmentService.CreateSegment(createSegmentDto);
        if (createdSegmentId == Guid.Empty)
            return BadRequest("Invalid segment");
        return Ok(createdSegmentId);
    }

    /// <summary>
    /// Добавляет случайный процент пользователей в сегмент.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <param name="percent">Процент пользователей, которых нужно добавить (от 0 до 100).</param>
    /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
    [HttpPost("random-users/{segmentId:guid}")]
    public async Task<IActionResult> AddSegmentForAnyPercentRandomVkUsers([FromRoute] Guid segmentId, [FromBody] int percent)
    {
        var result = await segmentService.AddSegmentForAnyPercentRandomVkUsers(segmentId, percent);
        return result ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Добавляет конкретных пользователей в сегмент.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <param name="userIds">Список идентификаторов пользователей, которых нужно добавить.</param>
    /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
    [HttpPost("users/{segmentId:guid}")]
    public async Task<IActionResult> AddSomeVkUsersForSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.AddSomeUsersForSegment(segmentId, userIds);
        return result ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получает список всех сегментов.
    /// </summary>
    /// <returns>Список всех сегментов.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSegments()
    {
        var segments = await segmentService.GetSegments();
        return Ok(segments);
    }

    /// <summary>
    /// Получает сегмент по его идентификатору.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <returns>Данные сегмента или 404, если он не найден.</returns>
    [HttpGet("{segmentId:guid}")]
    public async Task<IActionResult> GetSegment([FromRoute] Guid segmentId)
    {
        var segment = await segmentService.GetSegment(segmentId);
        return segment is not null ? Ok(segment) : NotFound();
    }

    /// <summary>
    /// Обновляет данные сегмента.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <param name="updateSegmentDto">Обновленные данные сегмента.</param>
    /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
    [HttpPatch("{segmentId:guid}")]
    public async Task<IActionResult> UpdateSegment([FromRoute] Guid segmentId, [FromBody] UpdateSegmentDto updateSegmentDto)
    {
        var result = await segmentService.UpdateSegment(segmentId, updateSegmentDto);
        return result ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Удаляет определенных пользователей из сегмента.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <param name="userIds">Список идентификаторов пользователей для удаления из сегмента.</param>
    /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
    [HttpDelete("users/{segmentId:guid}")]
    public async Task<IActionResult> DeleteSomeUsersFromSegment([FromRoute] Guid segmentId, [FromBody] List<Guid> userIds)
    {
        var result = await segmentService.DeleteSomeUsersFromSegment(segmentId, userIds);
        return result ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Удаляет сегмент по идентификатору.
    /// </summary>
    /// <param name="segmentId">Идентификатор сегмента.</param>
    /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
    [HttpDelete("{segmentId:guid}")]
    public async Task<IActionResult> DeleteSegment([FromRoute] Guid segmentId)
    {
        var result = await segmentService.DeleteSegment(segmentId);
        return result ? Ok(result) : BadRequest();
    }
}
