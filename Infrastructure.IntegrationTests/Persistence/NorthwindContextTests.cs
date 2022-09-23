using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Persistence;

namespace Infrastructure.IntegrationTests.Persistence
{
    public class NorthwindContextTests : IDisposable
    {
        private readonly NorthwindContext _sut;

        public NorthwindContextTests()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _sut = new NorthwindContext(options);

            var category = new Category
            {
                CategoryName = "NoteBook",
                Description = "Description"
            };
            
            _sut.Categories.Add(category);

            _sut.SaveChanges();
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}
