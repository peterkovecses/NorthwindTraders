using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Configurations
{
    public class SalesTotalsByAmountConfiguration : IEntityTypeConfiguration<SalesTotalsByAmount>
    {
        public void Configure(EntityTypeBuilder<SalesTotalsByAmount> builder)
        {
            builder.HasNoKey();

            builder.ToView("Sales Totals by Amount");

            builder.Property(e => e.CompanyName).HasMaxLength(40);

            builder.Property(e => e.OrderId).HasColumnName("OrderID");

            builder.Property(e => e.SaleAmount).HasColumnType("money");

            builder.Property(e => e.ShippedDate).HasColumnType("datetime");
        }
    }
}
