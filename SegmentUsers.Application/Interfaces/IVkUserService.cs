using SegmentUsers.Application.DTOs;

namespace SegmentUsers.Application.Interfaces;

public interface IVkUserService
{
    public Task<bool> AddToSegments(Guid userId, List<Guid> segmentIds);
    public Task<UserResponseDto?> GetVkUser(Guid userId);
    public Task<List<UserResponseDto>> GetVkUsers();
    public Task<bool> DeleteFromSegments(Guid userId, List<Guid> segmentIds);
}