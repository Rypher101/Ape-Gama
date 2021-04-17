using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Order_Product")]
    public partial class OrderProductModel
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }
        [Key]
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Column("qty", TypeName = "decimal(18, 2)")]
        public decimal Qty { get; set; }
        [Column("unit")]
        public int Unit { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderProducts")]
        public virtual OrderModel Order { get; set; }
        [ForeignKey(nameof(ProdId))]
        [InverseProperty(nameof(ProductModel.OrderProducts))]
        public virtual ProductModel Prod { get; set; }
    }
}
