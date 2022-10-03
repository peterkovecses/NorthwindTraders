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

        public async Task<PagedResponse<CategoryDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var categories = await _unitOfWork.Categories.GetAllAsync(paginationFilter);

            var response = _mapper.Map<IEnumerable<CategoryDto>>(categories).ToPagedResponse();

            if (paginationQuery == null)
            {
                return response;
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return response.SetPagination(paginationQuery, next, previous);
        }
         
        public async Task<Response<CategoryDto>> GetAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);

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
            var categoryInDb = await _unitOfWork.Categories.GetAsync(categoryDto.CategoryId);

            _mapper.Map(categoryDto, categoryInDb);
            await _unitOfWork.CompleteAsync();

            return categoryDto.ToResponse();
        }

        public async Task<Response<CategoryDto>> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(category).ToResponse();
        }

        public async Task<Response<IEnumerable<CategoryDto>>> DeleteRangeAsync(int[] ids)
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => ids.Contains(c.CategoryId));

            _unitOfWork.Categories.RemoveRange(categories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories).ToResponse();
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
