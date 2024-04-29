using System;
using System.Collections.Generic;
using dipl4.Models;
using Microsoft.EntityFrameworkCore;

namespace dipl4;

public partial class PapirusContext : DbContext
{
    public PapirusContext()
    {
    }

    public PapirusContext(DbContextOptions<PapirusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOPS;Database=Papirus;Trusted_Connection=True;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.Idmanager);

            entity.ToTable("Manager");

            entity.Property(e => e.Idmanager).HasColumnName("IDManager");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Idorder);

            entity.ToTable("Order");

            entity.Property(e => e.Idorder).HasColumnName("IDOrder");
            entity.Property(e => e.DateOfCreate).HasColumnType("datetime");
            entity.Property(e => e.DeadLine).HasColumnType("datetime");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Manager).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Order_Manager");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Order_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.IdordProd);

            entity.ToTable("Order_Product");

            entity.Property(e => e.IdordProd).HasColumnName("IDOrdProd");
            entity.Property(e => e.Amount).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Order_Product_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Order_Product_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Idproduct);

            entity.ToTable("Product");

            entity.Property(e => e.Idproduct).HasColumnName("IDProduct");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Idstatus);

            entity.ToTable("Status");

            entity.Property(e => e.Idstatus).HasColumnName("IDStatus");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("User");

            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.ClientContact).HasMaxLength(50);
            entity.Property(e => e.ClientName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
