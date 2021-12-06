using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ApeGama.Shared
{
    [Table("Notification")]
    public partial class NotificationModel
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("category")]
        public int Category { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Notifications")]
        public virtual UserModel User { get; set; }
    }
}
