using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ApeGama.Server.Models
{
    public partial class ApeGamaContext : DbContext
    {
        public ApeGamaContext()
        {
        }

        public ApeGamaContext(DbContextOptions<ApeGamaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<OnlineShop> OnlineShops { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#pragma warning disable CS1030 // #warning directive
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=RUSHI\\SQLEXPRESS;Initial Catalog=ApeGama;Integrated Security=True");
#pragma warning restore CS1030 // #warning directive
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<OnlineShop>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                    .HasName("PK__Online_S__74038772F8BFE59E");

                entity.ToTable("Online_Shop");

                entity.Property(e => e.ShopId).HasColumnName("shop_id");

                entity.Property(e => e.ShopAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("shop_address");

                entity.Property(e => e.ShopName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("shop_name");

                entity.Property(e => e.ShopTp)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("shop_tp");

                entity.Property(e => e.SupId).HasColumnName("sup_id");

                entity.HasOne(d => d.Sup)
                    .WithMany(p => p.OnlineShops)
                    .HasForeignKey(d => d.SupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Online_Shop_User");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CusId).HasColumnName("cus_id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderStatus)
                    .HasColumnType("numeric(1, 0)")
                    .HasColumnName("order_status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ReceivedDate)
                    .HasColumnType("date")
                    .HasColumnName("received_date");

                entity.Property(e => e.ShopId).HasColumnName("shop_id");

                entity.HasOne(d => d.Cus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_ToTable_1");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_ToTable");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProdId })
                    .HasName("PK__Order_Pr__46596229BA0BFA21");

                entity.ToTable("Order_Product");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProdId).HasColumnName("prod_id");

                entity.Property(e => e.Qty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("qty");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_product_ToTable");

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.ProdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_product_ToTable_1");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProdId)
                    .HasName("PK__Product__56958AB2AEA035EF");

                entity.ToTable("Product");

                entity.Property(e => e.ProdId).HasColumnName("prod_id");

                entity.Property(e => e.ProdDescription)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("prod_description");

                entity.Property(e => e.ProdName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("prod_name");

                entity.Property(e => e.ProdPrice)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("prod_price");

                entity.Property(e => e.ProdStatus)
                    .IsRequired()
                    .HasColumnName("prod_status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ProdStock).HasColumnName("prod_stock");

                entity.Property(e => e.ShopId).HasColumnName("shop_id");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ToTable");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("user_address");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("user_email");

                entity.Property(e => e.UserFlag)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("user_flag")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserPass)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasColumnName("user_pass")
                    .IsFixedLength(true);

                entity.Property(e => e.UserStatus)
                    .IsRequired()
                    .HasColumnName("user_status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserTp)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("user_tp");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
