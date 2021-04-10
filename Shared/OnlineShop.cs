using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Online_Shop")]
    public partial class OnlineShop
    {
        public OnlineShop()
        {
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
        }

        [Key]
        [Column("shop_id")]
        public int ShopId { get; set; }
        [Required]
        [Column("shop_name")]
        [StringLength(50)]
        public string ShopName { get; set; }
        [Required]
        [Column("shop_address")]
        [StringLength(100)]
        public string ShopAddress { get; set; }
        [Column("shop_tp", TypeName = "decimal(10, 0)")]
        public decimal? ShopTp { get; set; }
        [Column("sup_id")]
        public int SupId { get; set; }

        [ForeignKey(nameof(SupId))]
        [InverseProperty(nameof(User.OnlineShops))]
        public virtual User Sup { get; set; }
        [InverseProperty(nameof(Order.Shop))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Product.Shop))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
