namespace SegmentUsers.UI.DTOs;

public class NoteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Importance { get; set; }
    public bool IsFavorite { get; set; }

    public string FolderName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}