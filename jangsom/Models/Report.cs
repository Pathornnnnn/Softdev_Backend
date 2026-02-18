using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("reports")]
    public class Report
    {
        [Key]
        [Column("report_id")]
        public Guid ReportId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image_before_url")]
        public string ImageBeforeUrl { get; set; }

        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("asset_id")]
        public Guid? AssetId { get; set; }

        [Column("reporter_id")]
        public Guid ReporterId { get; set; }

        [Column("technician_id")]
        public Guid? TechnicianId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Location Location { get; set; }
        public Asset Asset { get; set; }
        public User Reporter { get; set; }
        public User Technician { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}