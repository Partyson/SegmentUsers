namespace SegmentUsers.Domain.Entities;

public class VkUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public Guid Id { get; set; }
    public List<Segment> Segments { get; set; } = null!;
} 