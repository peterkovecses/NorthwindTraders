using Northwind.Application.Exceptions;

namespace Application.UnitTests.Exceptions
{
    public class ItemNotFoundExceptionTests
    {
        [Fact]
        public void Constructor_WhenIdGiven_MessageAndPropertyAreSet()
        {
            // Arrange
            var id = 12;
            var expectedMessage = $"Item with Id {id} does not exist.";

            // Act
            var sut = new ItemNotFoundException<int>(id);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.Id.Should().Be(id);
        }
    }
}
