using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Complaint")]
    public partial class ComplaintModel
    {
        [Key]
        [Column("comp_id")]
        public int CompId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("order_id")]
        public int? OrderId { get; set; }
        [Required]
        [Column("details", TypeName = "text")]
        public string Details { get; set; }
        [Column("shop_id")]
        public int ShopId { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Column("status", TypeName = "numeric(1, 0)")]
        public decimal? Status { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("Complaints")]
        public virtual OrderModel Order { get; set; }
        [ForeignKey(nameof(ShopId))]
        [InverseProperty(nameof(OnlineShopModel.Complaints))]
        public virtual OnlineShopModel Shop { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Complaints")]
        public virtual UserModel User { get; set; }
    }
}
