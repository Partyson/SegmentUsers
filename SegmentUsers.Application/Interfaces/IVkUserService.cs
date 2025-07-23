using SegmentUsers.Application.DTOs;

namespace SegmentUsers.Application.Interfaces;

public interface IVkUserService
{
    public Task<Guid> CreateVkUser(CreateVkUserDto createVkUserDto);
    public Task<bool> AddToSegments(Guid vkUserId, List<Guid> segmentIds);
    public Task<VkUserResponseDto?> GetVkUser(Guid vkUserId);
    public Task<List<VkUserResponseDto>> GetVkUsers();
    public Task<bool> DeleteFromSegments(Guid vkUserId, List<Guid> segmentIds);
}