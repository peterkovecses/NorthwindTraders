using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Queries;
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

        public async Task<PagedResponse<ProductDto>> GetAsync(QueryParameters queryParameters)
        {
            var (totalItems, products) = await _unitOfWork.Products.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(totalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<ProductDto>>(products)
                .ToPagedResponse(queryParameters.Pagination, totalItems, next, previous);
        }

        public async Task<Response<ProductDto>> FindByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.FindByIdAsync(id);

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
            var productInDb = await _unitOfWork.Products.FindByIdAsync(productDto.ProductId);

            _mapper.Map(productDto, productInDb);
            await _unitOfWork.CompleteAsync();

            return productDto.ToResponse();
        }

        public async Task<Response<IEnumerable<ProductDto>>> DeleteAsync(int[] ids)
        {
            var products = (await _unitOfWork.Products.GetAsync(predicate: p => ids.Contains(p.ProductId))).items;

            _unitOfWork.Products.Remove(products);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Products.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Products.GetAsync(predicate: p => ids.Contains(p.ProductId))).items.Count() == ids.Length;
        }
    }
}
