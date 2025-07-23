namespace SegmentUsers.Domain.Entities;

public class Segment
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<VkUser> VkUsers { get; set; }
}