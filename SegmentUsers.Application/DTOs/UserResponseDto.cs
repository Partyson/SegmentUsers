namespace SegmentUsers.Application.DTOs;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public List<SegmentResponseDto> Segments { get; set; }
}