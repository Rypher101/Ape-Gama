using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApeGama.Server.Shared
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            OnlineShops = new HashSet<OnlineShop>();
            Orders = new HashSet<Order>();
        }

        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        [Required]
        [Column("user_name")]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [Column("user_pass")]
        [StringLength(64)]
        public string UserPass { get; set; }
        [Required]
        [Column("user_tp")]
        [StringLength(15)]
        public string UserTp { get; set; }
        [Required]
        [Column("user_address")]
        [StringLength(100)]
        public string UserAddress { get; set; }
        [Required]
        [Column("user_email")]
        [StringLength(30)]
        public string UserEmail { get; set; }
        [Column("user_flag", TypeName = "decimal(2, 0)")]
        public decimal UserFlag { get; set; }
        [Required]
        [Column("user_status")]
        public bool? UserStatus { get; set; }

        [InverseProperty(nameof(OnlineShop.Sup))]
        public virtual ICollection<OnlineShop> OnlineShops { get; set; }
        [InverseProperty(nameof(Order.Cus))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
