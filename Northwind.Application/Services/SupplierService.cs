using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
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

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto>? GetAsync(int id)
        {
            var suppliers = await _unitOfWork.Suppliers.GetAsync(id);

            return _mapper.Map<SupplierDto>(suppliers);
        }

        public async Task<int> CreateAsync(SupplierDto supplierDto)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            return supplier.SupplierId;
        }

        public async Task UpdateAsync(SupplierDto supplierDto)
        {
            var supplierInDb = await _unitOfWork.Suppliers.GetAsync(supplierDto.SupplierId);

            _mapper.Map(supplierDto, supplierInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<SupplierDto> DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetAsync(id);

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<IEnumerable<SupplierDto>> DeleteRangeAsync(int[] ids)
        {
            var suppliers = await _unitOfWork.Suppliers.FindAllAsync(s => ids.Contains(s.SupplierId));

            _unitOfWork.Suppliers.RemoveRange(suppliers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
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
