using Northwind.Application.Dtos;

namespace Northwind.Api.Models
{
    public class TransactionTestDto
    {
        public CategoryDto Category { get; set; }
        public ProductDto Product { get; set; }
    }
}
