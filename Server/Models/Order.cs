using System;
using System.Collections.Generic;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public int ShopId { get; set; }
        public int CusId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal OrderStatus { get; set; }

        public virtual User Cus { get; set; }
        public virtual OnlineShop Shop { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
