namespace Northwind.Domain.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees{ get; }
        Task<int> CompleteAsync();
    }
}
