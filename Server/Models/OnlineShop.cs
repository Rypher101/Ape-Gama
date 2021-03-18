using System;
using System.Collections.Generic;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class OnlineShop
    {
        public OnlineShop()
        {
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
        }

        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public decimal? ShopTp { get; set; }
        public int SupId { get; set; }

        public virtual User Sup { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
