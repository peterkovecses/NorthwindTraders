using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Configurations
{
    public class OrderSubtotalConfiguration : IEntityTypeConfiguration<OrderSubtotal>
    {
        public void Configure(EntityTypeBuilder<OrderSubtotal> builder)
        {
            builder.HasNoKey();

            builder.ToView("Order Subtotals");

            builder.Property(e => e.OrderId).HasColumnName("OrderID");

            builder.Property(e => e.Subtotal).HasColumnType("money");
        }
    }
}
