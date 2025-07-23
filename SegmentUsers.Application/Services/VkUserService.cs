using Microsoft.EntityFrameworkCore;
using SegmentUsers.Application.DTOs;
using SegmentUsers.Application.Interfaces;
using SegmentUsers.Infrastructure.Data;

namespace SegmentUsers.Application.Services;

public class VkUserService(AppDbContext context) : IVkUserService
{
    public async Task<bool> AddToSegments(Guid vkUserId, List<Guid> segmentIds)
    {
        var vkUser = await context.VkUsers
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == vkUserId);

        if (vkUser == null) 
            return false;

        var segments = await context.Segments
            .Where(s => segmentIds.Contains(s.Id) && vkUser.Segments.All(segment => segment.Id != s.Id))
            .ToListAsync();
        
        vkUser.Segments.AddRange(segments);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<VkUserResponseDto?> GetVkUser(Guid vkUserId)
    {
        var vkUser = await context.VkUsers
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == vkUserId);

        if (vkUser == null) 
            return null;

        return new VkUserResponseDto
        {
            Id = vkUser.Id,
            Email = vkUser.Email,
            LastName = vkUser.LastName,
            Name = vkUser.Name,
            Segments = vkUser.Segments.Select(s => new SegmentItemDto
            {
                Id = s.Id,
                Description = s.Description,
                Name = s.Name
            }).ToList(),
        };
    }

    public async Task<List<VkUserResponseDto>> GetVkUsers()
    {
        var vkUsers = await context.VkUsers
            .Select(u => new VkUserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                LastName = u.LastName,
                Name = u.Name,
                Segments = u.Segments.Select(s => new SegmentItemDto
                {
                    Id = s.Id,
                    Description = s.Description,
                    Name = s.Name
                }).ToList()
            })
            .ToListAsync();
        return vkUsers;
    }

    public async Task<bool> DeleteFromSegments(Guid vkUserId, List<Guid> segmentIds)
    {
        var vkUser = await context.VkUsers
            .Include(u => u.Segments)
            .FirstOrDefaultAsync(u => u.Id == vkUserId);

        if (vkUser == null)
            return false;

        vkUser.Segments.RemoveAll(s => segmentIds.Contains(s.Id));
        await context.SaveChangesAsync();
        return true;
    }
}