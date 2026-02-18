namespace jangsom.Models;

public class NotificationResponseDto
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
}