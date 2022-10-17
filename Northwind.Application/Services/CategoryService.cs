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
        private readonly IPaginatedUriService _uriService;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<CategoryDto>> GetAsync(QueryParameters<CategoryFilter> queryParameters)
        {
            if (queryParameters.Filter != null)
            {

            }

            else
            {

            }
            var result = await _unitOfWork.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<CategoryDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<CategoryDto>> FindByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.FindByIdAsync(id);

            return _mapper.Map<CategoryDto>(category).ToResponse();
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            categoryDto.CategoryId = category.CategoryId;

            return categoryDto.ToResponse();
        }

        public async Task<Response<CategoryDto>> UpdateAsync(CategoryDto categoryDto)
        {
            var categoryInDb = await _unitOfWork.Categories.FindByIdAsync(categoryDto.CategoryId);

            _mapper.Map(categoryDto, categoryInDb);
            await _unitOfWork.CompleteAsync();

            return categoryDto.ToResponse();
        }

        public async Task<Response<IEnumerable<CategoryDto>>> DeleteAsync(int[] ids)
        {
            var categories = (await _unitOfWork.Categories.GetAsync(predicate: c => ids.Contains(c.CategoryId))).Items;

            _unitOfWork.Categories.Remove(categories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Categories.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Categories.GetAsync(predicate: c => ids.Contains(c.CategoryId))).Items.Count() == ids.Length;
        }
    }
}
