namespace SegmentUsers.Application.DTOs;

public class CreateSegmentDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Guid> UserIds { get; set; }
}