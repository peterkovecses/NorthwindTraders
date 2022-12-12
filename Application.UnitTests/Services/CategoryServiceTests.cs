using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Application.Services;
using Northwind.Domain.Entities;
using System.Linq.Expressions;

namespace Application.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapper;

        public CategoryServiceTests()
        {
            _unitOfWorkMock= new Mock<IUnitOfWork>();
            _mapper= new Mock<IMapper>();            
        }

        [Fact]
        public async Task Get_WhenQueryParametersPasses_PagedResponseReturned()
        {
            // Arrange
            var queryParameters = new QueryParameters<CategoryFilter, Category>();

            IEnumerable<Category> categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "category1"},
                new Category { CategoryId = 2, CategoryName = "category2"},
                new Category { CategoryId = 3, CategoryName = "category3"}
            };
            var totalCategories = categories.Count();

            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, CategoryName = "category1"},
                new CategoryDto { CategoryId = 2, CategoryName = "category2"},
                new CategoryDto { CategoryId = 3, CategoryName = "category3"}
            };

            var expected = categoryDtos.ToPagedResponse(queryParameters.Pagination, totalCategories);

            _unitOfWorkMock
                .Setup(u => u.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, It.IsAny<Expression<Func<Category, bool>>>(), CancellationToken.None))
                .Returns(Task.FromResult((totalCategories, categories)));

            _mapper
                .Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            var sut = new CategoryService(_unitOfWorkMock.Object, _mapper.Object);

            // Act
            var actual = await sut.GetAsync(queryParameters);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task FindById_WhenExistingIdPassed_ResponseReturnedWithData()
        {
            // Arrange
            var id = 7;
            var category = new Category { CategoryId = id };
            var categoryDto = new CategoryDto { CategoryId = id };
            var expected = categoryDto.ToResponse();

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(id, CancellationToken.None))
                .Returns(Task.FromResult(category));

            _mapper
                .Setup(m => m.Map<CategoryDto>(category))
                .Returns(categoryDto);

            var sut = new CategoryService(_unitOfWorkMock.Object, _mapper.Object);

            // Act
            var actual = await sut.FindByIdAsync(id);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task FindById_WhenNonExistingIdPassed_ResponseReturnedWithoutData()
        {
            // Arrange
            var id = 7;
            Category category = null;
            var expected = new Response<CategoryDto>();
            var sut = new CategoryService(_unitOfWorkMock.Object, _mapper.Object);            

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(id, CancellationToken.None));

            _mapper
                .Setup(m => m.Map<CategoryDto>(category));

            // Act
            var actual = await sut.FindByIdAsync(id);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Create_WhenCategoryDtoPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var categoryDto = new CategoryDto { CategoryId = 12 };
            var category = new Category { CategoryId = 12 };
            var expected = categoryDto.ToResponse();

            _mapper
                .Setup(m => m.Map<Category>(categoryDto))
                .Returns(category);

            _unitOfWorkMock
                .Setup(u => u.Categories.AddAsync(category, CancellationToken.None));

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync());

            var sut = new CategoryService(_unitOfWorkMock.Object, _mapper.Object);

            // Act
            var actual = await sut.CreateAsync(categoryDto);

            // Assert
            _mapper.Verify(m => m.Map<Category>(categoryDto));
            _unitOfWorkMock.Verify(u => u.Categories.AddAsync(category, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Update_WhenCategoryDtoPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var categoryDto = new CategoryDto { CategoryId = 9 };
            var categoryInDb = new Category { CategoryId = 9 };
            var expected = categoryDto.ToResponse();

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None))
                .Returns(Task.FromResult(categoryInDb));

            _mapper
                .Setup(m => m.Map(categoryDto, categoryInDb));

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync());

            var sut = new CategoryService(_unitOfWorkMock.Object, _mapper.Object);

            // Act
            var actual = await sut.UpdateAsync(categoryDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None));
            _mapper.Verify(m => m.Map(categoryDto, categoryInDb));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
