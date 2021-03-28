using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ApeGama.Server.Shared
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
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

        [ForeignKey(nameof(ShopId))]
        [InverseProperty(nameof(OnlineShop.Products))]
        public virtual OnlineShop Shop { get; set; }
        [InverseProperty(nameof(OrderProduct.Prod))]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
