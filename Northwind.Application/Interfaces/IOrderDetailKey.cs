namespace Northwind.Application.Interfaces
{
    public interface IOrderDetailKey
    {
        int OrderId { get; set; }
        int ProductId { get; set; }
    }
}