using Northwind.Application.Models;

namespace Application.UnitTests.Models
{
    public class SortingTests
    {
        [Fact]
        public void Initializer_WhenSortByIsSet_IsNoSortingReturnsFalse()
        {
            // Arrange
            var sut = new Sorting { SortBy = "name" };

            // Act
            var actual = sut.IsNoSorting;

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void Initializer_WhenSortByIsNotSet_IsNoSortingReturnsTrue()
        {
            // Arrange
            var sut = new Sorting();

            // Act
            var actual = sut.IsNoSorting;

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void NoSorting_WhenCalled_ReturnsInstanceWithTheExpectedPropertyValues()
        {
            // Act
            var actual = Sorting.NoSorting();

            // Assert
            actual.SortBy.Should().BeNull();
            actual.DescendingOrder.Should().BeFalse();
            actual.IsNoSorting.Should().BeTrue();
        }
    }
}
