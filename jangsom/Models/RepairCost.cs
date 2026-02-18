using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jangsom.Models
{
    [Table("repair_costs")]
    public class RepairCost
    {
        [Key]
        [Column("cost_id")]
        public Guid CostId { get; set; }

        [Column("report_id")]
        public Guid ReportId { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; } // ใช้ decimal สำหรับตัวเลขคำนวณเงิน

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("supplier")]
        public string? Supplier { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}