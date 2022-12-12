using Northwind.Application.Extensions;
using Northwind.Application.Models;

namespace Application.UnitTests.Extensions
{
    public class ResponseExtensionsTests
    {
        [Fact]
        public void Response_WhenCalled_ReturnsTheWrappedData()
        {
            // Arrange
            var data = "data";
            var expected = new Response<string>(data);

            // Act
            var actual = data.ToResponse();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PagedResponse_WhenCalled_ReturnsTheWrappedData()
        {
            // Arrange
            var data = new List<string> { "data1", "data2", "data3" };
            var pagination = new Pagination();
            int totalItems = 50;
            var expected = new PagedResponse<string>(data, pagination, totalItems);

            // Act
            var actual = data.ToPagedResponse(pagination, totalItems);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
