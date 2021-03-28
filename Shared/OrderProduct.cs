using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ApeGama.Server.Shared
{
    [Table("Order_Product")]
    public partial class OrderProduct
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
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(ProdId))]
        [InverseProperty(nameof(Product.OrderProducts))]
        public virtual Product Prod { get; set; }
    }
}
