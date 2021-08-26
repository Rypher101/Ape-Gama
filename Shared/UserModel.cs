using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace ApeGama.Shared
{
    [Table("User")]
    [Microsoft.EntityFrameworkCore.Index(nameof(UserEmail), Name = "IX_User", IsUnique = true)]
    public partial class UserModel
    {
        public UserModel()
        {
            Orders = new HashSet<OrderModel>();
            Reviews = new HashSet<ReviewModel>();
        }

        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [Column("user_name")]
        [StringLength(50)]
        public string UserName { get; set; }
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
        [InverseProperty("Sup")]
        public virtual OnlineShopModel OnlineShop { get; set; }
        [InverseProperty(nameof(OrderModel.Cus))]
        public virtual ICollection<OrderModel> Orders { get; set; }
        [InverseProperty(nameof(ReviewModel.User))]
        public virtual ICollection<ReviewModel> Reviews { get; set; }

        public void ShaEnc(bool isNewPass = false)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                StringBuilder builder = new StringBuilder();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(UserPass));
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                UserPass = builder.ToString();
            }
            if (isNewPass)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    StringBuilder builder2 = new StringBuilder();
                    byte[] bytesNew = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(OldUserPass));
                    for (int i = 0; i < bytesNew.Length; i++)
                    {
                        builder2.Append(bytesNew[i].ToString("x2"));
                    }
                    OldUserPass = builder2.ToString();
                }
            }
        }

        [NotMapped]
        public bool passwordChange { get; set; } = false;
        [NotMapped]
        public string OldUserPass { get; set; }

    }
}
