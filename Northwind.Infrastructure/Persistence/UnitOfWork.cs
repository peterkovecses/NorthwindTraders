using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Infrastructure.Persistence.Repositories;

namespace Northwind.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindContext _context;

        public UnitOfWork(NorthwindContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            CustomerDemographics = new CustomerDemographicRepository(_context);
            Customers = new CustomerRepository(_context);
            Employees = new EmployeeRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
            Regions = new RegionRepository(_context);
            Shippers = new ShipperRepository(_context);
            Suppliers = new SupplierRepository(_context);
            Territories = new TerritoryRepository(_context);
        }

        public ICategoryRepository Categories { get; private set; }
        public ICustomerDemographicRepository CustomerDemographics { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IOrderDetailRepository OrderDetails { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public IRegionRepository Regions { get; private set; }
        public IShipperRepository Shippers { get; private set; }
        public ISupplierRepository Suppliers { get; private set; }
        public ITerritoryRepository Territories { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
