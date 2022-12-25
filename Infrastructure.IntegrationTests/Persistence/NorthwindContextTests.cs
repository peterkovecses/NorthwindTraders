using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Persistence;
using Northwind.Infrastructure.Persistence.Interceptors;

namespace Infrastructure.IntegrationTests.Persistence
{
    public class NorthwindContextTests : IDisposable
    {
        private readonly DateTime _dateTime;
        private readonly NorthwindContext _sut;

        public NorthwindContextTests()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dateTime = new DateTime(2500, 03, 07);
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(d => d.GetDateTime()).Returns(_dateTime);
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var auditInterceptor = new AuditInterceptor(dateTimeProviderMock.Object, currentUserServiceMock.Object);

            _sut = new NorthwindContext(options, auditInterceptor);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenNewCategoryAdded_CreatedPropertySet()
        {
            // Act
            var category = await AddCategory();

            // Assert
            category.Created.Should().Be(_dateTime);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenCategoryUpdated_ModifiedPropertiesSet()
        {
            // Arrange
            await AddCategory();
            var category = await _sut.Categories.FindAsync(1);
            category.Description = "Category descreption";

            // Act
            await _sut.SaveChangesAsync();
            
            category.LastModified.Should().Be(_dateTime);
        }

        private async Task<Category> AddCategory()
        {
            var category = new Category
            {
                CategoryName = "NoteBook",
                Description = "Description"
            };
            _sut.Categories.Add(category);
            await _sut.SaveChangesAsync();
            return category;
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}
