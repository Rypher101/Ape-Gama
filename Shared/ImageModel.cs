using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ApeGama.Server.Models
{
    [Table("Image")]
    public partial class ImageModel
    {
        [Key]
        [Column("img_id")]
        public int ImgId { get; set; }
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Required]
        [Column("img_name", TypeName = "text")]
        public string ImgName { get; set; }

        [ForeignKey(nameof(ProdId))]
        [InverseProperty(nameof(Product.Images))]
        public virtual Product Prod { get; set; }
    }
}
