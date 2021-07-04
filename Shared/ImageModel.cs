using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Image")]
    public partial class ImageModel
    {
        [Key]
        [Column("img_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImgId { get; set; }
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Required]
        [Column("img_name", TypeName = "text")]
        public string ImgName { get; set; }

        [ForeignKey(nameof(ProdId))]
        [InverseProperty(nameof(ProductModel.Images))]
        public virtual ProductModel Prod { get; set; }

        [NotMapped]
        public string fileString { get; set; }
        [NotMapped]
        public bool isDP { get; set; } = false;
    }
}
