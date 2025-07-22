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
        var users = context.Users
            .Where(x => createSegmentDto.UserIds.Contains(x.Id))
            .ToList();
        
        if (users.Count != createSegmentDto.UserIds.Count)
            return Guid.Empty;
        
        var segment = new Segment
        {
            Id = Guid.NewGuid(),
            Name = createSegmentDto.Name,
            Discription = createSegmentDto.Description,
            Users = users
        };
        
        var segmentEntityEntry = await context.Segments.AddAsync(segment);
        await context.SaveChangesAsync();
        return segmentEntityEntry.Entity.Id;
    }

    public async Task<bool> AddSegmentForAnyPercentRandomUsers(Guid segmentId, int percent)
    {
        if (percent is < 0 or > 100)
            return false;

        var segment = await context.Segments
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null)
            return false;

        var usersNotInSegment = await context.Users
            .Where(u => segment.Users.All(su => su.Id != u.Id))
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
        
        segment.Users.AddRange(selectedUsers);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddSomeUsersForSegment(Guid segmentId, List<Guid> userIds)
    {
        var segment = await context.Segments
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        var users = await context.Users
            .Where(u => userIds.Contains(u.Id) && segment.Users.All(user => user.Id != u.Id))
            .ToListAsync();
        
        segment.Users.AddRange(users);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<SegmentResponseDto> GetSegment(Guid segmentId)
    {
        var segment = await context.Segments
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) return null;

        return new SegmentResponseDto
        {
            Id = segment.Id,
            Name = segment.Name,
            Description = segment.Discription,
            UserIds = segment.Users.Select(u => u.Id).ToList()
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
                Description = s.Discription,
                UserIds = s.Users.Select(u => u.Id).ToList()
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateSegment(Guid segmentId, UpdateSegmentDto updateSegmentDto)
    {
        var segment = await context.Segments.FindAsync(segmentId);
        if (segment == null) 
            return false;

        segment.Name = updateSegmentDto.Name;
        segment.Discription = updateSegmentDto.Description;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSomeUsersFromSegment(Guid segmentId, List<Guid> userIds)
    {
        var segment = await context.Segments
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == segmentId);

        if (segment == null) 
            return false;

        segment.Users.RemoveAll(u => userIds.Contains(u.Id));
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