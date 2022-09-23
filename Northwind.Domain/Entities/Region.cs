using Northwind.Domain.Common;

namespace Northwind.Domain.Entities
{
    public partial class Region : EntityBase
    {
        public Region()
        {
            Territories = new HashSet<Territory>();
        }

        public int RegionId { get; set; }
        public string RegionDescription { get; set; } = null!;

        public virtual ICollection<Territory> Territories { get; private set; }
    }
}
