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
        var vkUsers = await context.VkUsers
            .Where(x => createSegmentDto.VkUserIds != null && createSegmentDto.VkUserIds.Contains(x.Id))
            .ToListAsync();

        if (createSegmentDto.VkUserIds != null && vkUsers.Count != createSegmentDto.VkUserIds.Count)
            return Guid.Empty;
        
        var segment = new Segment
        {
            Id = Guid.NewGuid(),
            Name = createSegmentDto.Name,
            Description = createSegmentDto.Description,
            VkUsers = vkUsers
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
        
        var existingUserIds = segment.VkUsers.Select(u => u.Id).ToHashSet();

        var vkUsersNotInSegment = await context.VkUsers
            .Where(u => !existingUserIds.Contains(u.Id))
            .ToListAsync();

        if (vkUsersNotInSegment.Count == 0)
            return false;

        var vkUsersToAddCount = (int)Math.Floor(vkUsersNotInSegment.Count * (percent / 100.0));

        if (vkUsersToAddCount == 0)
            return true;
        
        var random = new Random();
        var selectedVkUsers = vkUsersNotInSegment
            .OrderBy(_ => random.Next())
            .Take(vkUsersToAddCount)
            .ToList();
        
        segment.VkUsers.AddRange(selectedVkUsers);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddSomeUsersForSegment(Guid segmentId, List<Guid> vkUserIds)
    {
        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        var vkUserIdsInSegment = segment.VkUsers.Select(u => u.Id).ToHashSet();

        var vkUsers = await context.VkUsers
            .Where(u => vkUserIds.Contains(u.Id) && !vkUserIdsInSegment.Contains(u.Id))
            .ToListAsync();
        
        segment.VkUsers.AddRange(vkUsers);
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
            Users = segment.VkUsers.Select(x => new VkUserItemDto
            {
                Id = x.Id,
                Email = x.Email,
                LastName = x.LastName,
                Name = x.Name,
            }).ToList()
        };
    }

    public async Task<List<SegmentResponseDto>> GetSegments()
    {
        return await context.Segments
            .Select(s => new SegmentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Users = s.VkUsers.Select(x => new VkUserItemDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    LastName = x.LastName,
                    Name = x.Name,
                }).ToList()
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

    public async Task<bool> DeleteSomeUsersFromSegment(Guid segmentId, List<Guid> vkUserIds)
    {
        var segment = await context.Segments
            .Include(s => s.VkUsers)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        segment.VkUsers.RemoveAll(u => vkUserIds.Contains(u.Id));
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