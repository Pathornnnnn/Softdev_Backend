using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("locations")]
    public class Location
    {
        [Key]
        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("location_name")]
        public string LocationName { get; set; }

        [Column("building")]
        public string Building { get; set; }

        [Column("floor")]
        public string Floor { get; set; }

        [Column("room")]
        public string Room { get; set; }

        // Navigation properties
        public ICollection<Asset> Assets { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}