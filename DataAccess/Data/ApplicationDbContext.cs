using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Blog;
using System.Collections.Generic;

namespace DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }


    // Blog

    public DbSet<Post> Posts { get; set; }

    //Comment

    //Reservation
    public DbSet<RestaurantsGroup> RestaurantsGroups { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<TableReservation> TableReservations { get; set; }

    // Image
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Main Course" },
            new Category { CategoryId = 2, Name = "Steaks" },
            new Category { CategoryId = 3, Name = "Snacks" },
            new Category { CategoryId = 4, Name = "Desserts" },
            new Category { CategoryId = 5, Name = "Drinks" }
            );

        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                CompanyId = 1,
                Name = "Tech Solution",
                StreetAddress = "123 Tech St",
                City = "Tech City",
                PhoneNumber = "666999000"
            },
            new Company
            {
                CompanyId = 2,
                Name = "Vivid Books",
                StreetAddress = "99 Vid St",
                City = "Vid City",
                PhoneNumber = "3247439874"
            },
            new Company
            {
                CompanyId = 3,
                Name = "Readers Club",
                StreetAddress = "99 Main St",
                City = "Lala Land",
                PhoneNumber = "2343408237"
            }
            );

        modelBuilder.Entity<Product>().HasData(
                    new Product
                    {
                        ProductId = 1,
                        Name = "Cocoa Brownies",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 95,
                        SKU = "012",
                        CategoryId = 4
                    },
                    new Product
                    {
                        ProductId = 3,
                        Name = "Sausage Assortment",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 14,
                        SKU = "013",
                        CategoryId = 2
                    },
                    new Product
                    {
                        ProductId = 4,
                        Name = "Caesar With Chicken",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 11,
                        SKU = "Non",
                        CategoryId = 3
                    },
                    new Product
                    {
                        ProductId = 5,
                        Name = "Steak With Garnish",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 28,
                        SKU = "Non",
                        CategoryId = 2
                    },
                    new Product
                    {
                        ProductId = 6,
                        Name = "Club Sandwich",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 9.5,
                        SKU = "Non",
                        CategoryId = 1
                    },
                    new Product
                    {
                        ProductId = 7,
                        Name = "Panne Arabbiata",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 9,
                        SKU = "Non",
                        CategoryId = 4
                    },
                    new Product
                    {
                        ProductId = 8,
                        Name = "Kanafeh",
                        Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry",
                        Price = 9,
                        SKU = "Non",
                        CategoryId = 5
                    }
                    );
    }

}
