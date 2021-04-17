using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ApeGama.Shared;
using System.Configuration;

#nullable disable

namespace ApeGama.Server.Data
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

        public virtual DbSet<OnlineShopModel> OnlineShops { get; set; }
        public virtual DbSet<OrderModel> Orders { get; set; }
        public virtual DbSet<OrderProductModel> OrderProducts { get; set; }
        public virtual DbSet<ProductModel> Products { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["MainDB"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<OnlineShopModel>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                    .HasName("PK__Online_S__74038772F8BFE59E");

                entity.Property(e => e.ShopAddress).IsUnicode(false);

                entity.Property(e => e.ShopName).IsUnicode(false);

                entity.HasOne(d => d.Sup)
                    .WithMany(p => p.OnlineShops)
                    .HasForeignKey(d => d.SupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Online_Shop_User");
            });

            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.Property(e => e.OrderStatus).HasDefaultValueSql("((1))");

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

            modelBuilder.Entity<OrderProductModel>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProdId })
                    .HasName("PK__Order_Pr__46596229BA0BFA21");

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

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasKey(e => e.ProdId)
                    .HasName("PK__Product__56958AB2AEA035EF");

                entity.Property(e => e.ProdName).IsUnicode(false);

                entity.Property(e => e.ProdStatus).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ToTable");
            });

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(e => e.UserAddress).IsUnicode(false);

                entity.Property(e => e.UserEmail).IsUnicode(false);

                entity.Property(e => e.UserFlag).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserName).IsUnicode(false);

                entity.Property(e => e.UserPass)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.UserStatus).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserTp).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
