using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Services;

namespace SegmentUsers.Application.Interfaces;

public interface ISegmentService
{
    public Task<Guid> CreateSegment(CreateSegmentDto createSegmentDto);
    public Task<bool> AddSegmentForAnyPercentRandomVkUsers(Guid segmentId, int percent);
    public Task<bool> AddSomeUsersForSegment(Guid segmentId, List<Guid> userIds);
    public Task<SegmentResponseDto?> GetSegment(Guid segmentId);
    public Task<List<SegmentResponseDto>> GetSegments();
    public Task<bool> UpdateSegment(Guid segmentId, UpdateSegmentDto updateSegmentDto);
    public Task<bool> DeleteSomeUsersFromSegment(Guid segmentId, List<Guid> userIds);
    public Task<bool> DeleteSegment(Guid segmentId);
}