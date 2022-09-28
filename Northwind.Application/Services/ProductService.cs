using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var products = await _unitOfWork.Products.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto>? GetAsync(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<int> CreateAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return product.ProductId;
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            var productInDb = await _unitOfWork.Products.GetAsync(productDto.ProductId);

            _mapper.Map(productDto, productInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<ProductDto> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(id);

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> DeleteRangeAsync(int[] ids)
        {
            var products = await _unitOfWork.Products.FindAllAsync(p => ids.Contains(p.ProductId));

            _unitOfWork.Products.RemoveRange(products);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Products.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Products.FindAllAsync(p => ids.Contains(p.ProductId))).Count() == ids.Length;
        }
    }
}
