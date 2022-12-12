using Northwind.Application.Exceptions;
using Northwind.Application.Models;

namespace Application.UnitTests.Exceptions
{
    public class ValueAboveMaxPageSizeExceptionTests
    {
        [Fact]
        public void Constructor_WhenPageSizeGiven_MessageAndPropertyAreSet()
        {
            // Arrange
            var pageSize = 10;
            var expectedMessage = $"Page size {pageSize} is above the maximum page size ({Pagination.MaxPageSize}).";

            // Act
            var sut = new ValueAboveMaxPageSizeException(pageSize);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.PageSize.Should().Be(pageSize);
        }
    }
}
