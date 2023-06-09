﻿using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        public OrderRepository(NorthwindContext context) : base(context)
        {                
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
