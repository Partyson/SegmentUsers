namespace SegmentUsers.UI.DTOs;

public class SegmentResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<VkUserItemDto>? Users { get; set; }
}