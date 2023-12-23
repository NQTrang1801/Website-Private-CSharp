using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateWeb.Models
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string ShippingAddress { get; set; }

        [MaxLength(255)]
        public string OtherRequirements { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal StandardShipping { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal EstimatedTotal { get; set; }

        [MaxLength(50)]
        public string OrderStatus { get; set; }

        [MaxLength(450)]
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; }

        public virtual AspNetUser User { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
