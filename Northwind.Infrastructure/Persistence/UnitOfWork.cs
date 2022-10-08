using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Infrastructure.Persistence.Repositories;

namespace Northwind.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindContext _context;
        private readonly IStrategyResolver _strategyResolver;

        public UnitOfWork(NorthwindContext context, IStrategyResolver strategyResolver)
        {
            _context = context;
            _strategyResolver = strategyResolver;
            Categories = new CategoryRepository(_context, _strategyResolver);
            CustomerDemographics = new CustomerDemographicRepository(_context, _strategyResolver);
            Customers = new CustomerRepository(_context, _strategyResolver);
            Employees = new EmployeeRepository(_context, _strategyResolver);
            OrderDetails = new OrderDetailRepository(_context, _strategyResolver);
            Orders = new OrderRepository(_context, _strategyResolver);
            Products = new ProductRepository(_context, _strategyResolver);
            Regions = new RegionRepository(_context, _strategyResolver);
            Shippers = new ShipperRepository(_context, _strategyResolver);
            Suppliers = new SupplierRepository(_context, _strategyResolver);
            Territories = new TerritoryRepository(_context, _strategyResolver);
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
