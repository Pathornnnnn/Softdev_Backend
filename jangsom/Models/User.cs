using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("is_admin")]
        public bool IsAdmin { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Report> ReportsAsReporter { get; set; }
        public ICollection<Report> ReportsAsTechnician { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}