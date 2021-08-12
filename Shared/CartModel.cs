using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApeGama.Shared
{
    public class CartModel
    {
        public int prodID { get; set; }
        public int qty { get; set; }
        public string product { get; set; }
        public decimal price { get; set; }
        public string qtyString { get; set; }
        public bool isAvailable { get; set; }
        public int shopID { get; set; }
        public string shopName { get; set; }
        public int confirmShopID { get; set; }
        public int orderID { get; set; }
    }
}
