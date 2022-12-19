using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Application.UnitTests.Models
{
    public class ResponseTests
    {
        [Fact]
        public void Constructor_WhenNoArgumentPassed_PropertiesAreSetAsExpected()
        {
            // Act
            var sut = new Response<Category>();

            // Assert
            sut.Data.Should().BeNull();
            sut.HasData.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenArgumenPassedWithNullValue_PropertiesAreSetAsExpected()
        {
            // Arrange
            Category category = null;

            // Act
            var sut = new Response<Category>(category);

            // Assert
            sut.Data.Should().BeNull();
            sut.HasData.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenArgumenPassedWitNotNullValue_PropertiesAreSetAsExpected()
        {
            // Arrange
            var category = new Category();

            // Act
            var sut = new Response<Category>(category);

            // Assert
            sut.Data.Should().NotBeNull();
            sut.HasData.Should().BeTrue();
        }
    }
}
