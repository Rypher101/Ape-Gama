using System;
using System.Collections.Generic;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class User
    {
        public User()
        {
            OnlineShops = new HashSet<OnlineShop>();
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
        public string UserTp { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public decimal UserFlag { get; set; }
        public bool? UserStatus { get; set; }

        public virtual ICollection<OnlineShop> OnlineShops { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
