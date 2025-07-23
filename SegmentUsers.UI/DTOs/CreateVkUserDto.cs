namespace SegmentUsers.UI.DTOs;

public class CreateVkUserDto
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Guid>? SegmentIds { get; set; }
}