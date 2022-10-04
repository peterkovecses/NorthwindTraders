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

        public async Task<Response<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers).ToResponse();
        }

        public async Task<PagedResponse<SupplierDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, suppliers) = await _unitOfWork.Suppliers.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<SupplierDto>> GetAsync(int id)
        {
            var suppliers = await _unitOfWork.Suppliers.GetAsync(id);

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
            var supplierInDb = await _unitOfWork.Suppliers.GetAsync(supplierDto.SupplierId);
            _mapper.Map(supplierDto, supplierInDb);

            await _unitOfWork.CompleteAsync();

            return supplierDto.ToResponse();
        }

        public async Task<Response<SupplierDto>> DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetAsync(id);

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SupplierDto>(supplier).ToResponse();
        }

        public async Task<Response<IEnumerable<SupplierDto>>> DeleteRangeAsync(int[] ids)
        {
            var suppliers = await _unitOfWork.Suppliers.FindAllAsync(s => ids.Contains(s.SupplierId));

            _unitOfWork.Suppliers.RemoveRange(suppliers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Suppliers.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Suppliers.FindAllAsync(s => ids.Contains(s.SupplierId))).Count() == ids.Length;
        }
    }
}
