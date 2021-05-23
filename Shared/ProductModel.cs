using ApeGama.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Server.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            Images = new HashSet<ImageModel>();
            OrderProducts = new HashSet<OrderProductModel>();
            Reviews = new HashSet<ReviewModel>();
        }

        [Key]
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Required]
        [Column("prod_name")]
        [StringLength(50)]
        public string ProdName { get; set; }
        [Required]
        [Column("prod_description", TypeName = "text")]
        public string ProdDescription { get; set; }
        [Column("prod_stock")]
        public int ProdStock { get; set; }
        [Column("prod_price", TypeName = "decimal(8, 2)")]
        public decimal ProdPrice { get; set; }
        [Column("shop_id")]
        public int ShopId { get; set; }
        [Required]
        [Column("prod_status")]
        public bool? ProdStatus { get; set; }
        [Column("prod_dp", TypeName = "text")]
        public string ProdDp { get; set; }

        [ForeignKey(nameof(ShopId))]
        [InverseProperty(nameof(OnlineShopModel.Products))]
        public virtual OnlineShopModel Shop { get; set; }
        [InverseProperty(nameof(ImageModel.Prod))]
        public virtual ICollection<ImageModel> Images { get; set; }
        [InverseProperty(nameof(OrderProductModel.Prod))]
        public virtual ICollection<OrderProductModel> OrderProducts { get; set; }
        [InverseProperty(nameof(ReviewModel.Prod))]
        public virtual ICollection<ReviewModel> Reviews { get; set; }
    }
}
