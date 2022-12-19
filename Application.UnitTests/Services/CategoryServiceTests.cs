using AutoMapper;
using Northwind.Application.Dtos;
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
        public async Task Get_WhenQueryParametersPasses_ExpectedMethodsAreCalled()
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

            _unitOfWorkMock
                .Setup(u => u.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, It.IsAny<Expression<Func<Category, bool>>>(), CancellationToken.None))
                .ReturnsAsync((totalCategories, categories));

            _mapperMock
                .Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, It.IsAny<Expression<Func<Category, bool>>>(), CancellationToken.None));
            _mapperMock.Verify(m => m.Map<IEnumerable<CategoryDto>>(categories));
        }

        [Fact]
        public async Task FindById_WhenExistingIdPassed_ResponseReturnedWithData()
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
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(id, CancellationToken.None));
            _mapperMock.Verify(u => u.Map<CategoryDto>(category));
            actual.HasData.Should().BeTrue();
        }

        [Fact]
        public async Task FindById_WhenNonExistingIdPassed_ResponseReturnedWithoutData()
        {
            // Arrange
            var id = 7;
            Category category = null;

            _unitOfWorkMock
                .Setup(u => u.Categories.FindByIdAsync(id, CancellationToken.None));

            _mapperMock
                .Setup(m => m.Map<CategoryDto>(category));

            // Act
            var actual = await _sut.FindByIdAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(id, CancellationToken.None));
            _mapperMock.Verify(u => u.Map<CategoryDto>(category));
            actual.HasData.Should().BeFalse();
        }

        [Fact]
        public async Task Create_WhenCategoryDtoPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var categoryDto = new CategoryDto { CategoryId = 12 };
            var category = new Category { CategoryId = 12 };

            _mapperMock
                .Setup(m => m.Map<Category>(categoryDto))
                .Returns(category);

            _unitOfWorkMock
                .Setup(u => u.Categories.AddAsync(category, CancellationToken.None));

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync());

            // Act
            var actual = await _sut.CreateAsync(categoryDto);

            // Assert
            _mapperMock.Verify(m => m.Map<Category>(categoryDto));
            _unitOfWorkMock.Verify(u => u.Categories.AddAsync(category, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
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
            _unitOfWorkMock.Verify(u => u.Categories.FindByIdAsync(categoryDto.CategoryId, CancellationToken.None));
            _mapperMock.Verify(m => m.Map(categoryDto, categoryInDb));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
        }

        [Fact]
        public async Task Delete_WhenIdsPassed_ExpectedMethodsAreCalled()
        {
            // Arrange
            var ids = new[] { 45, 50 };
            IEnumerable<Category> categoriesToRemove = new List<Category>
            {
                new Category { CategoryId = 45 },
                new Category { CategoryId = 50 }

            };
            var totalCategories = categoriesToRemove.Count();

            IEnumerable<CategoryDto> categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 45 },
                new CategoryDto { CategoryId = 50 }

            };

            _unitOfWorkMock
                .Setup(u => u.Categories.GetAsync(It.IsAny<Pagination>(), It.IsAny<Sorting>(), c => ids.Contains(c.CategoryId), CancellationToken.None))
                .ReturnsAsync((totalCategories, categoriesToRemove));

            _unitOfWorkMock.Setup(u => u.Categories.Remove(It.IsAny<Category>()));
            _unitOfWorkMock.Setup(u => u.CompleteAsync());
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categoriesToRemove)).Returns(categoryDtos);

            // Act
            var actual = await _sut.DeleteAsync(ids);

            // Assert
            _unitOfWorkMock
                .Verify(u => u.Categories.GetAsync(It.IsAny<Pagination>(), It.IsAny<Sorting>(), c => ids.Contains(c.CategoryId), CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Categories.Remove(It.IsAny<Category>()));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(u => u.Map<IEnumerable<CategoryDto>>(categoriesToRemove));
        }
    }
}
