using AutoMapper;
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
            var (totalCategories, categories) = await _unitOfWork.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

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
            var categoryInDb = 
                await _unitOfWork.Categories.FindByIdAsync(categoryDto.CategoryId, token) ?? throw new ItemNotFoundException<int>(categoryDto.CategoryId);
            _mapper.Map(categoryDto, categoryInDb);
            await _unitOfWork.CompleteAsync();

            return categoryDto.ToResponse();
        }

        public async Task<Response<IEnumerable<CategoryDto>>> DeleteAsync(int[] ids, CancellationToken token = default )
        {
            var categoriesToRemove = (await _unitOfWork.Categories.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), c => ids.Contains(c.CategoryId), token)).items;

            foreach (var category in categoriesToRemove)
            {
                _unitOfWork.Categories.Remove(category);

            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categoriesToRemove).ToResponse();
        }
    }
}
