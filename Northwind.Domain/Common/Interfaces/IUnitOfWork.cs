using Northwind.Domain.Common.Interfaces.Repositories;

namespace Northwind.Domain.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public ICategoryRepository Categories { get; }
        public ICustomerDemographicRepository CustomerDemographics { get; }
        public ICustomerRepository Customers { get; }
        public IEmployeeRepository Employees { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public IRegionRepository Regions { get; }
        public IShipperRepository Shippers { get; }
        public ISupplierRepository Suppliers { get; }
        public ITerritoryRepository Territories { get; }

        Task<int> CompleteAsync();
    }
}
