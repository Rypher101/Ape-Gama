using System;
using System.Collections.Generic;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdDescription { get; set; }
        public int ProdStock { get; set; }
        public decimal ProdPrice { get; set; }
        public int ShopId { get; set; }
        public bool? ProdStatus { get; set; }

        public virtual OnlineShop Shop { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
