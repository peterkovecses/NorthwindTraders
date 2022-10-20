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

        public async Task<PagedResponse<SupplierDto>> GetAsync(QueryParameters<SupplierFilter> queryParameters, CancellationToken token = default)
        {
            var (totalShippers, shippers) = await _unitOfWork.Suppliers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(totalShippers);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<SupplierDto>>(shippers)
                .ToPagedResponse(queryParameters.Pagination, totalShippers, next, previous);
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
            var supplierInDb = await _unitOfWork.Suppliers.FindByIdAsync(supplierDto.SupplierId, token);
            _mapper.Map(supplierDto, supplierInDb);

            await _unitOfWork.CompleteAsync();

            return supplierDto.ToResponse();
        }

        public async Task<Response<IEnumerable<SupplierDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var suppliers = (await _unitOfWork.Suppliers.GetAsync(predicate: s => ids.Contains(s.SupplierId), token: token)).items;

            _unitOfWork.Suppliers.Remove(suppliers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers).ToResponse();
        }

        public async Task<bool> IsExists(int id, CancellationToken token = default)
        {
            return await _unitOfWork.Suppliers.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(int[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Suppliers.GetAsync(predicate: s => ids.Contains(s.SupplierId), token: token)).items.Count() == ids.Length;
        }
    }
}
