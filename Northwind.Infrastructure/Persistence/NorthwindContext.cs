using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Entities;
using System.Reflection;

namespace Northwind.Infrastructure.Persistence
{
    public partial class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        public DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CategorySalesFor1997> CategorySalesFor1997s { get; set; } = null!;
        public DbSet<CurrentProductList> CurrentProductLists { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; } = null!;
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; } = null!;
        public DbSet<OrderSubtotal> OrderSubtotals { get; set; } = null!;
        public DbSet<OrdersQry> OrdersQries { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductSalesFor1997> ProductSalesFor1997s { get; set; } = null!;
        public DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; } = null!;    
        public DbSet<ProductsByCategory> ProductsByCategories { get; set; } = null!;
        public DbSet<QuarterlyOrder> QuarterlyOrders { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<SalesByCategory> SalesByCategories { get; set; } = null!;
        public DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; } = null!;
        public DbSet<Shipper> Shippers { get; set; } = null!;
        public DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; } = null!;
        public DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Territory> Territories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    }
}
