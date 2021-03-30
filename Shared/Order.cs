using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Server.Shared
{
    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("shop_id")]
        public int ShopId { get; set; }
        [Column("cus_id")]
        public int CusId { get; set; }
        [Column("order_date", TypeName = "date")]
        public DateTime OrderDate { get; set; }
        [Column("received_date", TypeName = "date")]
        public DateTime? ReceivedDate { get; set; }
        [Column("order_status", TypeName = "numeric(1, 0)")]
        public decimal OrderStatus { get; set; }

        [ForeignKey(nameof(CusId))]
        [InverseProperty(nameof(User.Orders))]
        public virtual User Cus { get; set; }
        [ForeignKey(nameof(ShopId))]
        [InverseProperty(nameof(OnlineShop.Orders))]
        public virtual OnlineShop Shop { get; set; }
        [InverseProperty(nameof(OrderProduct.Order))]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
