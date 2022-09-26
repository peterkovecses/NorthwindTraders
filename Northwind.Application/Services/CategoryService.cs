using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var categories = await _unitOfWork.Categories.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
         
        public async Task<CategoryDto>? GetAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<int> CreateAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return category.CategoryId;
        }

        public async Task UpdateAsync(CategoryDto categoryDto)
        {
            var categoryInDb = await _unitOfWork.Categories.GetAsync(categoryDto.CategoryId);

            _mapper.Map(categoryDto, categoryInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<CategoryDto> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> DeleteRangeAsync(int[] ids)
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => ids.Contains(c.CategoryId));

            _unitOfWork.Categories.RemoveRange(categories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Categories.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Categories.FindAllAsync(c => ids.Contains(c.CategoryId))).Count() == ids.Length;
        }
    }
}
