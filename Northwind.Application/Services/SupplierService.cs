using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;
using System.Linq.Expressions;

namespace Northwind.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SupplierDto>> GetAsync(QueryParameters<SupplierFilter> queryParameters, CancellationToken token = default)
        {
            Expression<Func<Supplier, bool>> predicate = x => true;
            var (totalShippers, shippers) = await _unitOfWork.Suppliers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, predicate, token);

            return _mapper.Map<IEnumerable<SupplierDto>>(shippers)
                .ToPagedResponse(queryParameters.Pagination, totalShippers);
        }

        public async Task<Response<SupplierDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var suppliers = await _unitOfWork.Suppliers.FindByIdAsync(id, token);

            return _mapper.Map<SupplierDto>(suppliers).ToResponse();
        }

        public async Task<Response<SupplierDto>> CreateAsync(SupplierDto supplierDto, CancellationToken token = default)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);

            await _unitOfWork.Suppliers.AddAsync(supplier, token);
            await _unitOfWork.CompleteAsync();

            supplierDto.SupplierId = supplier.SupplierId;

            return supplierDto.ToResponse();
        }

        public async Task<Response<SupplierDto>> UpdateAsync(SupplierDto supplierDto, CancellationToken token = default)
        {
            var supplierInDb = 
                await _unitOfWork.Suppliers.FindByIdAsync(supplierDto.SupplierId, token) ?? throw new ItemNotFoundException(supplierDto.SupplierId);
            _mapper.Map(supplierDto, supplierInDb);
            await _unitOfWork.CompleteAsync();

            return supplierDto.ToResponse();
        }

        public async Task<Response<IEnumerable<SupplierDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var suppliersToRemove = (await _unitOfWork.Suppliers.GetAsync(new Pagination(), new Sorting(), s => ids.Contains(s.SupplierId), token)).items;

            foreach (var supplier in suppliersToRemove)
            {
            _unitOfWork.Suppliers.Remove(supplier);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliersToRemove).ToResponse();
        }
    }
}
