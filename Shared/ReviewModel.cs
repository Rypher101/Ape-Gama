﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Review")]
    public partial class ReviewModel
    {
        [Key]
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("review", TypeName = "text")]
        public string Review1 { get; set; }
        [Column("rate")]
        public int Rate { get; set; }

        [ForeignKey(nameof(ProdId))]
        [InverseProperty(nameof(ProductModel.Reviews))]
        public virtual ProductModel Prod { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Reviews")]
        public virtual UserModel User { get; set; }
    }
}
