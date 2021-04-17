﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Product")]
    public partial class ProductModel
    {
        public ProductModel()
        {
            OrderProducts = new HashSet<OrderProductModel>();
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
        [InverseProperty(nameof(OnlineShopModel.Products))]
        public virtual OnlineShopModel Shop { get; set; }
        [InverseProperty(nameof(OrderProductModel.Prod))]
        public virtual ICollection<OrderProductModel> OrderProducts { get; set; }
    }
}