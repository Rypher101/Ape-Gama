using ApeGama.Shared;
using Microsoft.EntityFrameworkCore;
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

        public virtual DbSet<ComplaintModel> Complaints { get; set; }
        public virtual DbSet<ImageModel> Images { get; set; }
        public virtual DbSet<OnlineShopModel> OnlineShops { get; set; }
        public virtual DbSet<OrderModel> Orders { get; set; }
        public virtual DbSet<OrderProductModel> OrderProducts { get; set; }
        public virtual DbSet<ProductModel> Products { get; set; }
        public virtual DbSet<ReviewModel> Reviews { get; set; }
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

            modelBuilder.Entity<ComplaintModel>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Complaint_Order");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Complaint_Online_Shop");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Complaint_User");
            });

            modelBuilder.Entity<ImageModel>(entity =>
            {
                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ProdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Image_Product");
            });

            modelBuilder.Entity<OnlineShopModel>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                    .HasName("PK__Online_S__74038772F8BFE59E");

                entity.Property(e => e.ShopAddress).IsUnicode(false);

                entity.Property(e => e.ShopName).IsUnicode(false);

                entity.Property(e => e.ShopTp).IsUnicode(false);

                entity.HasOne(d => d.Sup)
                    .WithOne(p => p.OnlineShop)
                    .HasForeignKey<OnlineShopModel>(d => d.SupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Online_Shop_User");
            });

            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.Property(e => e.OrderAddress).IsUnicode(false);

                entity.Property(e => e.OrderContact).IsUnicode(false);

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

            modelBuilder.Entity<ReviewModel>(entity =>
            {
                entity.HasKey(e => new { e.ProdId, e.UserId });

                entity.Property(e => e.Rate).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_Product");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_User");
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
