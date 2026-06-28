using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KinetiqueAPI.Models
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("customer_id")]
        public Guid CustomerId { get; set; } // 👈 Changed from string to Guid

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
