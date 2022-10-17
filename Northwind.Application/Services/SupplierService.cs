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
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<SupplierDto>> GetAsync(QueryParameters<SupplierFilter> queryParameters)
        {
            var result = await _unitOfWork.Suppliers.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<SupplierDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<SupplierDto>> FindByIdAsync(int id)
        {
            var suppliers = await _unitOfWork.Suppliers.FindByIdAsync(id);

            return _mapper.Map<SupplierDto>(suppliers).ToResponse();
        }

        public async Task<Response<SupplierDto>> CreateAsync(SupplierDto supplierDto)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            supplierDto.SupplierId = supplier.SupplierId;

            return supplierDto.ToResponse();
        }

        public async Task<Response<SupplierDto>> UpdateAsync(SupplierDto supplierDto)
        {
            var supplierInDb = await _unitOfWork.Suppliers.FindByIdAsync(supplierDto.SupplierId);
            _mapper.Map(supplierDto, supplierInDb);

            await _unitOfWork.CompleteAsync();

            return supplierDto.ToResponse();
        }

        public async Task<Response<IEnumerable<SupplierDto>>> DeleteAsync(int[] ids)
        {
            var suppliers = (await _unitOfWork.Suppliers.GetAsync(predicate: s => ids.Contains(s.SupplierId))).Items;

            _unitOfWork.Suppliers.Remove(suppliers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Suppliers.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Suppliers.GetAsync(predicate: s => ids.Contains(s.SupplierId))).Items.Count() == ids.Length;
        }
    }
}
