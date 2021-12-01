using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Online_Shop")]
    public partial class OnlineShopModel
    {
        public OnlineShopModel()
        {
            Orders = new HashSet<OrderModel>();
            Products = new HashSet<ProductModel>();
        }

        [Key]
        [Column("shop_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShopId { get; set; }
        [Required]
        [Column("shop_name")]
        [StringLength(50)]
        public string ShopName { get; set; }
        [Required]
        [Column("shop_address")]
        [StringLength(100)]
        public string ShopAddress { get; set; }
        [Column("shop_tp")]
        [StringLength(15)]
        public string ShopTp { get; set; }
        [Column("sup_id")]
        public int SupId { get; set; }

        [ForeignKey(nameof(SupId))]
        [InverseProperty(nameof(UserModel.OnlineShop))]
        public virtual UserModel Sup { get; set; }
        [InverseProperty(nameof(OrderModel.Shop))]
        public virtual ICollection<OrderModel> Orders { get; set; }
        [InverseProperty(nameof(ProductModel.Shop))]
        public virtual ICollection<ProductModel> Products { get; set; }
        [InverseProperty(nameof(ComplaintModel.Shop))]
        public virtual ICollection<ComplaintModel> Complaints { get; set; }
    }
}
