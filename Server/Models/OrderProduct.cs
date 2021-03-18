using System;
using System.Collections.Generic;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class OrderProduct
    {
        public int OrderId { get; set; }
        public int ProdId { get; set; }
        public decimal Qty { get; set; }
        public int Unit { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Prod { get; set; }
    }
}
