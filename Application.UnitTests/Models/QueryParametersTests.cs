using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Application.UnitTests.Models
{
    public class QueryParametersTests
    {
        [Fact]
        public void Constructor_WhenNoValueGivenToProperties_PropertiesAreInitializedWithDefaultValues()
        {
            // Act
            var sut = new QueryParameters<CategoryFilter, Category>();

            // Assert
            sut.Pagination.Should().BeEquivalentTo(Pagination.DefaultPagination());
            sut.Sorting.Should().BeEquivalentTo(Sorting.NoSorting());
            sut.Filter.Should().BeEquivalentTo(new CategoryFilter());
        }
    }
}
