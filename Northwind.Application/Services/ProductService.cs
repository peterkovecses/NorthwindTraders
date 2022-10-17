using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
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

        public async Task<PagedResponse<ProductDto>> GetAsync(QueryParameters<ProductFilter> queryParameters, CancellationToken token = default)
        {
            var result = await _unitOfWork.Products.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<ProductDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<ProductDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var product = await _unitOfWork.Products.FindByIdAsync(id, token);

            return _mapper.Map<ProductDto>(product).ToResponse();
        }

        public async Task<Response<ProductDto>> CreateAsync(ProductDto productDto, CancellationToken token = default)
        {
            var product = _mapper.Map<Product>(productDto);

            await _unitOfWork.Products.AddAsync(product, token);
            await _unitOfWork.CompleteAsync();

            productDto.ProductId = product.ProductId;

            return productDto.ToResponse();
        }

        public async Task<Response<ProductDto>> UpdateAsync(ProductDto productDto, CancellationToken token = default)
        {
            var productInDb = await _unitOfWork.Products.FindByIdAsync(productDto.ProductId, token);

            _mapper.Map(productDto, productInDb);
            await _unitOfWork.CompleteAsync();

            return productDto.ToResponse();
        }

        public async Task<Response<IEnumerable<ProductDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var products = (await _unitOfWork.Products.GetAsync(predicate: p => ids.Contains(p.ProductId), token: token)).Items;

            _unitOfWork.Products.Remove(products);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products).ToResponse();
        }

        public async Task<bool> IsExists(int id, CancellationToken token = default)
        {
            return await _unitOfWork.Products.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(int[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Products.GetAsync(predicate: p => ids.Contains(p.ProductId), token: token)).Items.Count() == ids.Length;
        }
    }
}
