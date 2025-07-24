using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;

namespace SegmentUsers.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class VkUserController(IVkUserService vkUserService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateVkUser(CreateVkUserDto createVkUserDto)
    {
        var createdVkUserId = await vkUserService.CreateVkUser(createVkUserDto);
        return createdVkUserId != Guid.Empty ? Ok(createdVkUserId) : BadRequest("Invalid User");
    }
    
    /// <summary>
    /// Добавить пользователя в указанные сегменты.
    /// </summary>
    /// <param name="vkUserId">ID пользователя VK.</param>
    /// <param name="segmentIds">Список ID сегментов, в которые нужно добавить пользователя.</param>
    /// <returns>Результат выполнения операции.</returns>
    [HttpPost("{vkUserId:guid}")]
    public async Task<IActionResult> AddToSegments([FromRoute] Guid vkUserId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.AddToSegments(vkUserId, segmentIds);
        return Ok(result);
    }

    /// <summary>
    /// Получить информацию о пользователе VK по его ID.
    /// </summary>
    /// <param name="vkUserId">ID пользователя VK.</param>
    /// <returns>Объект пользователя или 404, если не найден.</returns>
    [HttpGet("{vkUserId:guid}")]
    public async Task<IActionResult> GetVkUser([FromRoute] Guid vkUserId)
    {
        var user = await vkUserService.GetVkUser(vkUserId);
        return user is not null ? Ok(user): NotFound();
    }

    /// <summary>
    /// Получить список всех пользователей VK.
    /// </summary>
    /// <returns>Список всех пользователей VK.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllVkUsers()
    {
        var vkUsers = await vkUserService.GetVkUsers();
        return Ok(vkUsers);
    }
    
    /// <summary>
    /// Удалить пользователя из указанных сегментов.
    /// </summary>
    /// <param name="vkUserId">ID пользователя VK.</param>
    /// <param name="segmentIds">Список ID сегментов, из которых нужно удалить пользователя.</param>
    /// <returns>Результат выполнения операции.</returns>
    [HttpDelete("{vkUserId:guid}")]
    public async Task<IActionResult> DeleteFromSegments([FromRoute] Guid vkUserId, [FromBody] List<Guid> segmentIds)
    {
        var result = await vkUserService.DeleteFromSegments(vkUserId, segmentIds);
        return Ok(result);
    }
}