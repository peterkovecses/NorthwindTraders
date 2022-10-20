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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CategoryDto>> GetAsync(QueryParameters<CategoryFilter> queryParameters, CancellationToken token = default)
        {
            var (totalCategories, categories) = await _unitOfWork.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(totalCategories);

            return _mapper.Map<IEnumerable<CategoryDto>>(categories)
                .ToPagedResponse(queryParameters.Pagination, totalCategories);
        }

        public async Task<Response<CategoryDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var category = await _unitOfWork.Categories.FindByIdAsync(id, token);

            return _mapper.Map<CategoryDto>(category).ToResponse();
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto, CancellationToken token = default)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _unitOfWork.Categories.AddAsync(category, token);
            await _unitOfWork.CompleteAsync();

            categoryDto.CategoryId = category.CategoryId;

            return categoryDto.ToResponse();
        }

        public async Task<Response<CategoryDto>> UpdateAsync(CategoryDto categoryDto, CancellationToken token = default)
        {
            var categoryInDb = await _unitOfWork.Categories.FindByIdAsync(categoryDto.CategoryId, token);

            _mapper.Map(categoryDto, categoryInDb);
            await _unitOfWork.CompleteAsync();

            return categoryDto.ToResponse();
        }

        public async Task<Response<IEnumerable<CategoryDto>>> DeleteAsync(int[] ids, CancellationToken token = default )
        {
            var categories = (await _unitOfWork.Categories.GetAsync(predicate: c => ids.Contains(c.CategoryId), token: token)).items;

            _unitOfWork.Categories.Remove(categories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories).ToResponse();
        }

        public async Task<bool> IsExists(int id, CancellationToken token = default)
        {
            return await _unitOfWork.Categories.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(int[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Categories.GetAsync(predicate: c => ids.Contains(c.CategoryId), token: token)).items.Count() == ids.Length;
        }
    }
}
