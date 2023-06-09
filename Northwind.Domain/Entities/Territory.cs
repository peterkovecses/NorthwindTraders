﻿using Northwind.Domain.Common;

namespace Northwind.Domain.Entities
{
    public partial class Territory : EntityBase
    {
        public Territory()
        {
            Employees = new HashSet<Employee>();
        }

        public string TerritoryId { get; set; } = null!;
        public string TerritoryDescription { get; set; } = null!;

        public int RegionId { get; set; }
        public virtual Region Region { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; private set; }
    }
}
