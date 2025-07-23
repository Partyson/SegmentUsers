using Microsoft.EntityFrameworkCore;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;
using SegmentUsers.Domain.Entities;
using SegmentUsers.Infrastructure.Data;

namespace SegmentUsers.Application.Services;

public class SegmentService(AppDbContext context) : ISegmentService
{
    public async Task<Guid> CreateSegment(CreateSegmentDto createSegmentDto)
    {
        var users = context.VkUsers
            .Where(x => createSegmentDto.VkUserIds.Contains(x.Id))
            .ToList();

        if (users.Count != createSegmentDto.VkUserIds.Count)
            return Guid.Empty;
        
        var segment = new Segment
        {
            Id = Guid.NewGuid(),
            Name = createSegmentDto.Name,
            Description = createSegmentDto.Description,
            VkUsers = users
        };
        
        var segmentEntityEntry = await context.Segments.AddAsync(segment);
        await context.SaveChangesAsync();
        return segmentEntityEntry.Entity.Id;
    }

    public async Task<bool> AddSegmentForAnyPercentRandomVkUsers(Guid segmentId, int percent)
    {
        //TODO перенести в валидацию
        if (percent is < 0 or > 100)
            return false;

        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null)
            return false;

        var usersNotInSegment = await context.VkUsers
            .Where(u => segment.VkUsers.All(su => su.Id != u.Id))
            .ToListAsync();

        if (usersNotInSegment.Count == 0)
            return false;

        var usersToAddCount = (int)Math.Floor(usersNotInSegment.Count * (percent / 100.0));

        if (usersToAddCount == 0)
            return true;
        
        var random = new Random();
        var selectedUsers = usersNotInSegment
            .OrderBy(_ => random.Next())
            .Take(usersToAddCount)
            .ToList();
        
        segment.VkUsers.AddRange(selectedUsers);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddSomeUsersForSegment(Guid segmentId, List<Guid> userIds)
    {
        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        var users = await context.VkUsers
            .Where(u => userIds.Contains(u.Id) && segment.VkUsers.All(user => user.Id != u.Id))
            .ToListAsync();
        
        segment.VkUsers.AddRange(users);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<SegmentResponseDto?> GetSegment(Guid segmentId)
    {
        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return null;

        return new SegmentResponseDto
        {
            Id = segment.Id,
            Name = segment.Name,
            Description = segment.Description,
        };
    }

    public async Task<List<SegmentResponseDto>> GetSegments()
    {
        return await context.Segments
            //.Include(s => s.Users)
            .Select(s => new SegmentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateSegment(Guid segmentId, UpdateSegmentDto updateSegmentDto)
    {
        var segment = await context.Segments.FindAsync(segmentId);
        if (segment == null) 
            return false;

        segment.Name = updateSegmentDto.Name;
        segment.Description = updateSegmentDto.Description;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSomeUsersFromSegment(Guid segmentId, List<Guid> userIds)
    {
        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        segment.VkUsers.RemoveAll(u => userIds.Contains(u.Id));
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSegment(Guid segmentId)
    {
        var segment = await context.Segments.FindAsync(segmentId);
        if (segment == null) return false;

        context.Segments.Remove(segment);
        await context.SaveChangesAsync();
        return true;
    }
}