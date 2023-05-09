using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
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
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoryService _sut;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _sut = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Get_WhenQueryParametersPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var queryParameters = new QueryParameters<CategoryFilter, Category>();

            IEnumerable<Category> categories = new List<Category>();
            var totalCategories = categories.Count();

            var categoryDtos = new List<CategoryDto>();

            _unitOfWorkMock
                .Setup(u => u.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, It.IsAny<Expression<Func<Category, bool>>>(), CancellationToken.None))
                .ReturnsAsync((totalCategories, categories));

            _mapperMock
                .Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, It.IsAny<Expression<Func<Category, bool>>>(), CancellationToken.None), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<CategoryDto>>(categories), Times.Once);
        }

        [Fact]
        public async Task FindById_WhenIdPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var id = 7;
            var category = new Category { CategoryId = id };
            var categoryDto = new CategoryDto { CategoryId = id };

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(id, CancellationToken.None))
                .ReturnsAsync(category);

            _mapperMock
                .Setup(m => m.Map<CategoryDto>(category))
                .Returns(categoryDto);

            // Act
            var actual = await _sut.FindByIdAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(id, CancellationToken.None), Times.Once);
            _mapperMock.Verify(u => u.Map<CategoryDto>(category), Times.Once);
        }

        [Fact]
        public async Task Create_WhenCategoryDtoPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var categoryDto = new CategoryDto();
            var category = new Category();
            var expectedId = 12;

            _mapperMock
                .Setup(m => m.Map<Category>(categoryDto))
                .Returns(category);

            _unitOfWorkMock
                .Setup(u => u.Categories.AddAsync(category, CancellationToken.None));

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync())
                .Callback(() => category.CategoryId = expectedId);

            // Act
            var actual = await _sut.CreateAsync(categoryDto);

            // Assert
            _mapperMock.Verify(m => m.Map<Category>(categoryDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.Categories.AddAsync(category, CancellationToken.None), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            actual.Data.CategoryId.Should().Be(expectedId);
        }

        [Fact]
        public async Task Update_WhenCategoryDtoPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var categoryDto = new CategoryDto { CategoryId = 9 };
            var categoryInDb = new Category { CategoryId = 9 };

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None))
                .ReturnsAsync(categoryInDb);

            _mapperMock
                .Setup(m => m.Map(categoryDto, categoryInDb));

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync());

            // Act
            var actual = await _sut.UpdateAsync(categoryDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None), Times.Once);
            _mapperMock.Verify(m => m.Map(categoryDto, categoryInDb), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_WhenItemNotFound_ThrowsItemNotFoundException()
        {
            // Arrange
            var categoryDto = new CategoryDto { CategoryId = 9 };
            Category? categoryInDb = default;

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None))
                .Returns(Task.FromResult(categoryInDb));

            // Act
            var act = () => _sut.UpdateAsync(categoryDto);

            // Assert
            await act.Should().ThrowAsync<ItemNotFoundException<int>>();
        }

        [Fact]
        public async Task Delete_WhenIdsPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var id = 1;
            var categoryToRemove = new Category { CategoryId = id };
            _unitOfWorkMock.Setup(u => u.Categories.FindByIdAsync(id, CancellationToken.None)).ReturnsAsync(categoryToRemove);

            // Act
            await _sut.DeleteAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(id, CancellationToken.None), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
