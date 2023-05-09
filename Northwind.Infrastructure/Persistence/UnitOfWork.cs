using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Infrastructure.Persistence.Repositories;

namespace Northwind.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindContext _context;
        private readonly Lazy<ICategoryRepository> _categories;
        private readonly Lazy<ICustomerDemographicRepository> _customerDemographics;
        private readonly Lazy<ICustomerRepository> _customers;
        private readonly Lazy<IEmployeeRepository> _employees;
        private readonly Lazy<IOrderDetailRepository> _orderDetails;
        private readonly Lazy<IOrderRepository> _orders;
        private readonly Lazy<IProductRepository> _products;
        private readonly Lazy<IRegionRepository> _regions;
        private readonly Lazy<IShipperRepository> _shippers;
        private readonly Lazy<ISupplierRepository> _suppliers;
        private readonly Lazy<ITerritoryRepository> _territories;

        public UnitOfWork(NorthwindContext context)
        {
            _context = context;
            _categories = new(() => new CategoryRepository(_context));
            _customerDemographics = new(() => new CustomerDemographicRepository(_context));
            _customers = new(() => new CustomerRepository(_context));
            _employees = new(() => new EmployeeRepository(_context));
            _orderDetails = new(() => new OrderDetailRepository(_context));
            _orders = new(() => new OrderRepository(_context));
            _products = new(() => new ProductRepository(_context));
            _regions = new(() => new RegionRepository(_context));
            _shippers = new(() => new ShipperRepository(_context));
            _suppliers = new(() => new SupplierRepository(_context));
            _territories = new(() => new TerritoryRepository(_context));
        }

        public ICategoryRepository Categories => _categories.Value;
        public ICustomerDemographicRepository CustomerDemographics => _customerDemographics.Value;
        public ICustomerRepository Customers => _customers.Value;
        public IEmployeeRepository Employees => _employees.Value;
        public IOrderDetailRepository OrderDetails => _orderDetails.Value;
        public IOrderRepository Orders => _orders.Value;
        public IProductRepository Products => _products.Value;
        public IRegionRepository Regions => _regions.Value;
        public IShipperRepository Shippers => _shippers.Value;
        public ISupplierRepository Suppliers => _suppliers.Value;
        public ITerritoryRepository Territories => _territories.Value;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
