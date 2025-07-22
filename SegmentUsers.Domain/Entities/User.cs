using Microsoft.AspNetCore.Identity;

namespace SegmentUsers.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public List<Segment> Segments { get; set; } = null!;
} 