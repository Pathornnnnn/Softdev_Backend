using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public Guid NotificationId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("related_report_id")]
        public Guid RelatedReportId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Report Report { get; set; }
    }
}