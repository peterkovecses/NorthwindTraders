using Newtonsoft.Json;
using Northwind.Application.Exceptions;

namespace Application.UnitTests.Extensions
{
    public class PropertyNotFoundExceptionTests
    {        
        private const string PropertyName = "Name";
        private const string SpecificMessage = $"Property {PropertyName} does not exist.";

        [Fact]
        public void Constructor_WhenNoArgs_MessageIsSetToDefault()
        {
            // Arrange
            var expectedMessage = "Property does not exist.";

            // Act
            var sut = new PropertyNotFoundException();

            // Assert
            sut.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Constructor_WhenPropertyNameGiven_MessageAndPropertyNameAreSet()
        {
            // Act
            var sut = new PropertyNotFoundException(PropertyName);

            // Assert
            sut.Message.Should().Be(SpecificMessage);
            sut.PropertyName.Should().Be(PropertyName);
        }

        [Fact]
        public void Constructor_WhenPropertyNameAndInnerExceptionGiven_MessageAndInnerExceptionAreSet()
        {
            // Arrange
            var innerException = new Exception();

            // Act
            var sut = new PropertyNotFoundException(PropertyName, innerException);

            // Assert
            sut.Message.Should().Be(SpecificMessage);
            sut.PropertyName.Should().Be(PropertyName);
            sut.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void Constructor_WhenPropertyNameGivenAndSerialized_DeserializeCorrectly()
        {
            // Arrange
            var sut = new PropertyNotFoundException(PropertyName);

            // Act
            var json = JsonConvert.SerializeObject(sut);
            var deserializedResult = JsonConvert.DeserializeObject<PropertyNotFoundException>(json);

            // Assert
            deserializedResult.PropertyName.Should().Be(PropertyName);
            deserializedResult.ToString().Should().BeEquivalentTo(sut.ToString());
        }

        [Fact]
        public void Constructor_WhenPropertyNameAndInnerExceptionGivenAndSerialized_DeserializeCorrectly()
        {
            // Arrange
            var innerException = new Exception();
            var sut = new PropertyNotFoundException(PropertyName, innerException);

            // Act
            var json = JsonConvert.SerializeObject(sut);
            var deserializedResult = JsonConvert.DeserializeObject<PropertyNotFoundException>(json);

            // Assert
            var expectedJson = @"{""PropertyName"":""Name"",""Message"":""Property Name does not exist."",""TargetSite"":null,""Data"":{},""InnerException"":{""TargetSite"":null,""Message"":""Exception of type 'System.Exception' was thrown."",""Data"":{},""InnerException"":null,""HelpLink"":null,""Source"":null,""HResult"":-2146233088,""StackTrace"":null},""HelpLink"":null,""Source"":null,""HResult"":-2146233088,""StackTrace"":null}";
            json.Should().Be(expectedJson);
            deserializedResult.PropertyName.Should().Be(PropertyName);
            deserializedResult.Message.Should().Be(SpecificMessage);
        }
    }
}
