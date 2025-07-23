namespace SegmentUsers.Application.DTOs;

public class CreateVkUserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<Guid>? SegmentIds { get; set; }
}