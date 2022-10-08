using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class OrderDetailKey : IOrderDetailKey
    {
        public OrderDetailKey(int orderId, int productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }

        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
