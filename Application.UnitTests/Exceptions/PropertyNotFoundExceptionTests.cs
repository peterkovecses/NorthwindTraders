using Northwind.Application.Exceptions;

namespace Application.UnitTests.Exceptions
{
    public class PropertyNotFoundExceptionTests
    {        
        [Fact]
        public void Constructor_WhenPropertyNameGiven_MessageAndPropertyNameAreSet()
        {
            // Arrange
            var propertyName = "Name";
            var expectedMessage = GetExpectedMessage(propertyName);

            // Act
            var sut = new PropertyNotFoundException(propertyName);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.PropertyName.Should().Be(propertyName);
        }

        [Fact]
        public void Constructor_WhenNullParameterGiven_MessageIsSet()
        {
            // Arrange
            string propertyName = null;
            var expectedMessage = GetExpectedMessage(propertyName);

            // Act
            var sut = new PropertyNotFoundException(propertyName);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.PropertyName.Should().Be(propertyName);
        }

        [Fact]
        public void Constructor_WhenEmptyStringGiven_MessageAndPropertyNameAreSet()
        {
            // Arrange
            var propertyName = string.Empty;
            var expectedMessage = GetExpectedMessage(propertyName);

            // Act
            var sut = new PropertyNotFoundException(propertyName);

            // Assert
            sut.Message.Should().Be(expectedMessage);
            sut.PropertyName.Should().Be(propertyName);
        }

        private static string GetExpectedMessage(string propertyName)
        {
            return $"Property {propertyName} does not exist.";
        }
    }
}
