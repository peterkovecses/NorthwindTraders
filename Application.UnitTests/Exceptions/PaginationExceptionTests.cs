using Northwind.Application.Exceptions;

namespace Application.UnitTests.Exceptions
{
    public class PaginationExceptionTests
    {
        [Fact]
        public void Constructor_WhenPageNumberGiven_MessageAndPropertyAreSet()
        {
            // Arrange
            var pageNumber = 12;
            var expectedMessage = $"Page number {pageNumber} does not exist.";

            // Act
            var sut = new PaginationException(pageNumber);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.PageNumber.Should().Be(pageNumber);
        }
    }
}
