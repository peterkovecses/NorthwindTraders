﻿using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
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

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ProductDto>> GetAsync(QueryParameters<ProductFilter, Product> queryParameters, CancellationToken token = default)
        {
            var (totalProducts, products) = await _unitOfWork.Products.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

            return _mapper.Map<IEnumerable<ProductDto>>(products)
                .ToPagedResponse(queryParameters.Pagination, totalProducts);
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
            var productInDb = 
                await _unitOfWork.Products.FindByIdAsync(productDto.ProductId, token) ?? throw new ItemNotFoundException<int>(productDto.ProductId);
            _mapper.Map(productDto, productInDb);
            await _unitOfWork.CompleteAsync();

            return productDto.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken token = default)
        {
            var productToRemove = await _unitOfWork.Products.FindByIdAsync(id, token);
            _unitOfWork.Products.Remove(productToRemove);

            await _unitOfWork.CompleteAsync();
        }
    }
}
