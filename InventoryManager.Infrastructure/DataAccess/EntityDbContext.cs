using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Models.Identity;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace InventoryManager.Infrastructure.DataAccess
{
    public class EntityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {

        //Tables
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInstance> ProductInstances { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<PropertyInstance> Properties { get; set; }
        public DbSet<Product_Property> Product_Propertys { get; set; }


        public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            //Join table for ProductInstance_Property

            modelBuilder.Entity<Product_Property>().HasKey(e => new { e.ProductId, e.PropertyId });
            
            modelBuilder.Entity<Product_Property>()
                .HasOne(e => e.Product)
                .WithMany(e => e.Product_Property)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product_Property>()
                .HasOne(e => e.Property)
                .WithMany(e => e.Product_Property)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);




            // PRODUCT TABLE


            modelBuilder.Entity<Product>().HasKey(e => e.Id);

            modelBuilder.Entity<Product>()
                .HasIndex(e => e.ProductNumber)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductNumber)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasIndex(e => e.ProductName)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductName)
                .HasMaxLength(22)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasIndex(e => e.ProductTypeId);

            //delete the foreign key if the productype gets removed.
            modelBuilder.Entity<Product>()
                .HasOne(e => e.ProductType)
                .WithMany()
                .HasForeignKey(e => e.ProductTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            //concurrency property
            modelBuilder.Entity<Product>()
                .Property(e => e.ConcurrencyStamp)
                .IsRowVersion();

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18, 2)");



            //PRODUCTTYPE TABLE



            modelBuilder.Entity<ProductType>().HasKey(e => e.Id);

            modelBuilder.Entity<ProductType>()
                .Property(e => e.ConcurrencyStamp)
                .IsRowVersion();

            modelBuilder.Entity<ProductType>()
                .HasIndex(e => e.Name)
                .IsUnique();

            modelBuilder.Entity<ProductType>()
                .Property(e => e.Name)
                .HasMaxLength(22);




            //PRODUCTINSTANCE TABLE



            modelBuilder.Entity<ProductInstance>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<ProductInstance>()
               .HasIndex(e => e.Barcode)
               .IsUnique();

           modelBuilder.Entity<ProductInstance>()
                .Property(e => e.PurchasePrice)
                .HasColumnType("decimal(18, 2)");
            
            modelBuilder.Entity<ProductInstance>()
                .Property(e => e.PurchasePrice)
                .IsRequired();

            modelBuilder.Entity<ProductInstance>()
               .Property(e => e.Status)
               .HasMaxLength(20)
               .IsRequired();

            modelBuilder.Entity<ProductInstance>()
               .Property(e => e.ConcurrencyStamp)
               .IsRowVersion();

            //foreign Keys and Navigation

            modelBuilder.Entity<ProductInstance>()
                .HasIndex(e => e.ProductId);

            modelBuilder.Entity<ProductInstance>()
                .HasIndex(e => e.LocationId);

            modelBuilder.Entity<ProductInstance>()
               .HasOne(e => e.Product)
               .WithMany()
               .HasForeignKey(e => e.ProductId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductInstance>()
               .HasOne(e => e.Location)
               .WithMany()
               .HasForeignKey(e => e.LocationId)
               .OnDelete(DeleteBehavior.SetNull);


            //LOCATION TABLE

            modelBuilder.Entity<Location>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Location>()
                .Property(e => e.ConcurrencyStamp)
                .IsRowVersion();


            modelBuilder.Entity<Location>()
                .Property(e => e.Name)
                .HasMaxLength(22)
                .IsRequired();


            //PROPERTYTYPE TABLE

            modelBuilder.Entity<PropertyType>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<PropertyType>()
                .Property (e => e.Name)
                .HasMaxLength(22)
                .IsRequired();

            modelBuilder.Entity<PropertyType>()
                .Property(e => e.ConcurrencyStamp)
                .IsRowVersion();


            //PROPERTYINSTANCE TABLE

            modelBuilder.Entity<Core.Models.PropertyInstance>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<PropertyInstance>()
                .Property(e => e.Name)
                .HasMaxLength(22)
                .IsRequired();


            //Navigation and foreign key
            modelBuilder.Entity<Core.Models.PropertyInstance>()
                .HasOne(e => e.PropertyType)
                .WithMany()
                .HasForeignKey(e => e.PropertyTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Core.Models.PropertyInstance>()
                .HasIndex(e => e.PropertyTypeId);

        }


    }
}
