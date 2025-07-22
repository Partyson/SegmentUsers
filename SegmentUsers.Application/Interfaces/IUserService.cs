using SegmentUsers.Application.DTOs;

namespace SegmentUsers.Application.Interfaces;

public interface IUserService
{
    public Task<bool> AddToSegments(Guid userId, List<Guid> segmentIds);
    public Task<List<SegmentResponseDto>> GetUserSegments(Guid userId);
    public Task<List<SegmentResponseDto>> GetAllUserSegments();
    public Task<bool> DeleteFromSegments(Guid userId, List<Guid> segmentIds);
}