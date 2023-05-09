using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Models;
using Northwind.Application.Interfaces.Services;
using System.Transactions;

namespace Northwind.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTestController : ApiControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public TransactionTestController(ICategoryService categoryService, IProductService productService, ILogger<CustomersController> logger) : base(logger)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        [HttpPost]
        public async Task<ActionResult> TestTransaction(TransactionTestDto foo, CancellationToken token)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _categoryService.CreateAsync(foo.Category, token);
            await _productService.CreateAsync(foo.Product, token);
            transaction.Complete();

            return Ok(); 
        }
    }
}


