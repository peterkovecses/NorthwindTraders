using Northwind.Domain.Common;

namespace Northwind.Domain.Entities
{
    public partial class Shipper : EntityBase
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual ICollection<Order> Orders { get; private set; }
    }
}
