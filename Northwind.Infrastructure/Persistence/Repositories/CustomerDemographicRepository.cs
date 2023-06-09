﻿using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class CustomerDemographicRepository : GenericRepository<CustomerDemographic, string>, ICustomerDemographicRepository
    {
        public CustomerDemographicRepository(NorthwindContext context) : base(context)
        {

        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
