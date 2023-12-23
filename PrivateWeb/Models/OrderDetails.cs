using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateWeb.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        public int OrderID { get; set; }

        public int VariantID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        [ForeignKey("Variantss")]
        public int VariantId { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Variantss Variant { get; set; }
    }
}
