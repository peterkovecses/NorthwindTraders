using Northwind.Domain.Common;

namespace Northwind.Domain.Entities
{
    public partial class CustomerDemographic : EntityBase
    {
        public CustomerDemographic()
        {
            Customers = new HashSet<Customer>();
        }

        public string CustomerTypeId { get; set; } = null!;
        public string? CustomerDesc { get; set; }

        public virtual ICollection<Customer> Customers { get; private set; }
    }
}
