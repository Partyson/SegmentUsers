namespace SegmentUsers.UI.DTOs;

public class VkUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<SegmentDto> Segments { get; set; } = new(); // важно!
}