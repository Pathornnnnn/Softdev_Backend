using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("assets")]
    public class Asset
    {
        [Key]
        [Column("asset_id")]
        public Guid AssetId { get; set; }

        [Column("asset_name")]
        public string AssetName { get; set; }

        [Column("asset_number")]
        public string AssetNumber { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Location Location { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}