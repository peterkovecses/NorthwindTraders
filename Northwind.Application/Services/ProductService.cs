using AutoMapper;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;
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
        private readonly IPaginatedUriService _uriService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<ProductDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var products = await _unitOfWork.Products.GetAllAsync(paginationFilter);

            var response = _mapper.Map<IEnumerable<ProductDto>>(products).ToPagedResponse();

            if (paginationQuery == null)
            {
                return response;
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return response.SetPagination(paginationQuery, next, previous);
        }

        public async Task<Response<ProductDto>> GetAsync(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(id);

            return _mapper.Map<ProductDto>(product).ToResponse();
        }

        public async Task<Response<ProductDto>> CreateAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            productDto.ProductId = product.ProductId;

            return productDto.ToResponse();
        }

        public async Task<Response<ProductDto>> UpdateAsync(ProductDto productDto)
        {
            var productInDb = await _unitOfWork.Products.GetAsync(productDto.ProductId);

            _mapper.Map(productDto, productInDb);
            await _unitOfWork.CompleteAsync();

            return productDto.ToResponse();
        }

        public async Task<Response<ProductDto>> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(id);

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(product).ToResponse();
        }

        public async Task<Response<IEnumerable<ProductDto>>> DeleteRangeAsync(int[] ids)
        {
            var products = await _unitOfWork.Products.FindAllAsync(p => ids.Contains(p.ProductId));

            _unitOfWork.Products.RemoveRange(products);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products).ToResponse();
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
