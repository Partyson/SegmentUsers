namespace SegmentUsers.UI.DTOs;

public class NoteInput
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Importance { get; set; }
    public bool IsFavorite { get; set; }
    public Guid? FolderId { get; set; }
}