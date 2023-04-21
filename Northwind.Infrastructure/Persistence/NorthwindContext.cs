using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Persistence.Interceptors;
using System.Reflection;

namespace Northwind.Infrastructure.Persistence
{
    public partial class NorthwindContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;

        public NorthwindContext(DbContextOptions<NorthwindContext> options, AuditInterceptor auditInterceptor)
            : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<Shipper> Shippers { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Territory> Territories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }
    }
}
