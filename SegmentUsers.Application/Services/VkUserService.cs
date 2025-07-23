using Microsoft.EntityFrameworkCore;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;
using SegmentUsers.Infrastructure.Data;

namespace SegmentUsers.Application.Services;

public class VkUserService(AppDbContext context) : IVkUserService
{
    public async Task<bool> AddToSegments(Guid userId, List<Guid> segmentIds)
    {
        var user = await context.VkUsers
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

    public async Task<UserResponseDto?> GetVkUser(Guid userId)
    {
        var user = await context.VkUsers
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) 
            return null;

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            LastName = user.LastName,
            Name = user.Name,
            Segments = user.Segments.Select(s => new SegmentResponseDto
            {
                Id = s.Id,
                Description = s.Description,
                Name = s.Name
            }).ToList(),
        };
    }

    public async Task<List<UserResponseDto>> GetVkUsers()
    {
        var users = await context.VkUsers
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                LastName = u.LastName,
                Name = u.Name,
                Segments = u.Segments.Select(s => new SegmentResponseDto
                {
                    Id = s.Id,
                    Description = s.Description,
                    Name = s.Name
                }).ToList()
            })
            .ToListAsync();
        return users;
    }

    public async Task<bool> DeleteFromSegments(Guid userId, List<Guid> segmentIds)
    {
        var user = await context.VkUsers
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return false;

        user.Segments.RemoveAll(s => segmentIds.Contains(s.Id));
        await context.SaveChangesAsync();
        return true;
    }
}