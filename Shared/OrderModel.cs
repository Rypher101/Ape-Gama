using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Order")]
    public partial class OrderModel
    {
        public OrderModel()
        {
            OrderProducts = new HashSet<OrderProductModel>();
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
        [InverseProperty(nameof(UserModel.Orders))]
        public virtual UserModel Cus { get; set; }
        [ForeignKey(nameof(ShopId))]
        [InverseProperty(nameof(OnlineShopModel.Orders))]
        public virtual OnlineShopModel Shop { get; set; }
        [InverseProperty(nameof(OrderProductModel.Order))]
        public virtual ICollection<OrderProductModel> OrderProducts { get; set; }
    }
}
