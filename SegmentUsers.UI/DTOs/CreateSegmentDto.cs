namespace SegmentUsers.UI.DTOs;

public class CreateSegmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid>? VkUserIds { get; set; }
}