namespace Northwind.Domain.Common.Models
{
    public class OrderDetailKey
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
