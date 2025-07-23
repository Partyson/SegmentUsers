namespace SegmentUsers.Application.DTOs;

public class SegmentResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}