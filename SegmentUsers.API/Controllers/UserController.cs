using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SegmentUsers.Application.Interfaces;

namespace SegmentUsers.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController(IVkUserService vkUserService) : ControllerBase
{
    /// <summary>
    /// Добавить пользователя в указанные сегменты.
    /// </summary>
    /// <param name="userId">ID пользователя VK.</param>
    /// <param name="segmentIds">Список ID сегментов, в которые нужно добавить пользователя.</param>
    /// <returns>Результат выполнения операции.</returns>
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddToSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.AddToSegments(userId, segmentIds);
        return Ok(result);
    }

    /// <summary>
    /// Получить информацию о пользователе VK по его ID.
    /// </summary>
    /// <param name="userId">ID пользователя VK.</param>
    /// <returns>Объект пользователя или 404, если не найден.</returns>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetVkUser([FromRoute] Guid userId)
    {
        var user = await vkUserService.GetVkUser(userId);
        return user is not null ? Ok(user): NotFound();
    }

    /// <summary>
    /// Получить список всех пользователей VK.
    /// </summary>
    /// <returns>Список всех пользователей VK.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllVkUsers()
    {
        var users = await vkUserService.GetVkUsers();
        return Ok(users);
    }
    
    /// <summary>
    /// Удалить пользователя из указанных сегментов.
    /// </summary>
    /// <param name="userId">ID пользователя VK.</param>
    /// <param name="segmentIds">Список ID сегментов, из которых нужно удалить пользователя.</param>
    /// <returns>Результат выполнения операции.</returns>
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteFromSegments([FromRoute] Guid userId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.DeleteFromSegments(userId, segmentIds);
        return Ok(result);
    }
}