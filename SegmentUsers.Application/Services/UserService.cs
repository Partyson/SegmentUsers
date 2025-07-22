using Microsoft.EntityFrameworkCore;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;
using SegmentUsers.Infrastructure.Data;

namespace SegmentUsers.Application.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<bool> AddToSegments(Guid userId, List<Guid> segmentIds)
    {
        var user = await context.Users
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) 
            return false;

        var segments = await context.Segments
            .Where(s => segmentIds.Contains(s.Id) && user.Segments.All(segment => segment.Id != s.Id))
            .ToListAsync();
        
        user.Segments.AddRange(segments);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SegmentResponseDto>> GetUserSegments(Guid userId)
    {
        var user = await context.Users
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) 
            return [];

        return user.Segments.Select(s => new SegmentResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Discription,
            UserIds = [user.Id]
        }).ToList();
    }

    public async Task<List<SegmentResponseDto>> GetAllUserSegments()
    {
        var users = await context.Users
            .Include(u => u.Segments)
            .ToListAsync();

        var result = new List<SegmentResponseDto>();

        foreach (var user in users)
        {
            result.AddRange(user.Segments
                .Select(segment => new SegmentResponseDto
                {
                    Id = segment.Id,
                    Name = segment.Name,
                    Description = segment.Discription,
                    UserIds = [user.Id]
                }));
        }

        return result;
    }

    public async Task<bool> DeleteFromSegments(Guid userId, List<Guid> segmentIds)
    {
        var user = await context.Users
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return false;

        user.Segments.RemoveAll(s => segmentIds.Contains(s.Id));
        await context.SaveChangesAsync();
        return true;
    }
}