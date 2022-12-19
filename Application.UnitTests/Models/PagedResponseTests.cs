using Northwind.Application.Models;

namespace Application.UnitTests.Models
{
    public class PagedResponseTests
    {
        [Fact]
        public void Constructor_WhenDataIsNull_HasDataReturnsFalse()
        {
            // Arrange
            IEnumerable<int> data = null;
            var totalItems = 0;

            // Act
            var actual = new PagedResponse<int>(data, Pagination.NoPagination(), totalItems).HasData;

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenDataIsNotNull_HasDataReturnsTrue()
        {
            // Arrange
            IEnumerable<int> data = new List<int> { 34, 78, 100 };
            var totalItems = 3;

            // Act
            var actual = new PagedResponse<int>(data, Pagination.NoPagination(), totalItems).HasData;

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WhenTotalItemsArgumentIsZero_TotalPagesIsSetToOne()
        {
            // Arrange
            IEnumerable<int> data = null;
            var totalItems = 0;
            var expected = 1;

            // Act
            var actual = new PagedResponse<int>(data, Pagination.NoPagination(), totalItems).TotalPages;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Constructor_WhenTotalItemsArgumentIsNotZero_TotalPagesIsSetToExpected()
        {
            // Arrange
            IEnumerable<string> data = new List<string> { "data1", "data2" };
            var totalItems = 44;
            var pagination = new Pagination { PageNumber = 10, PageSize = 2 };
            var expected = (int)Math.Ceiling((double)totalItems / pagination.PageSize);

            // Act
            var actual = new PagedResponse<string>(data, pagination, totalItems).TotalPages;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Initializer_WhenThereIsNoNextPage_NextPagePropertyIsSetToNull()
        {
            // Arrange
            IEnumerable<string> data = new List<string> { "data1", "data2" };
            var totalItems = 6;
            var pagination = new Pagination { PageNumber = 3, PageSize = 2 };

            // Act
            var sut = new PagedResponse<string>(data, pagination, totalItems)
            {                
                NextPage = "nextPage"
            };

            // Assert
            sut.NextPage.Should().BeNull();
        }

        [Fact]
        public void Initializer_WhenThereIsNextPage_NextPagePropertyIsSetToValue()
        {
            // Arrange
            IEnumerable<string> data = new List<string> { "data1", "data2" };
            var totalItems = 6;
            var pagination = new Pagination { PageNumber = 2, PageSize = 2 };
            var expected = "nextPage";

            // Act
            var sut = new PagedResponse<string>(data, pagination, totalItems)
            {
                NextPage = expected
            };

            // Assert
            sut.NextPage.Should().Be(expected);
        }
    }
}
