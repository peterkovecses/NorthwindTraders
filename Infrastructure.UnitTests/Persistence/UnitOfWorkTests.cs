using Microsoft.EntityFrameworkCore;
using Northwind.Infrastructure.Persistence;

namespace Infrastructure.UnitTests.Persistence
{
    public class UnitOfWorkTests
    {
        private readonly Mock<NorthwindContext> _mockContext;
        private readonly UnitOfWork _sut;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptions<NorthwindContext>();
            _mockContext = new Mock<NorthwindContext>(options);

            _sut = new UnitOfWork(_mockContext.Object);
        }

        [Fact]
        public async Task Complete_WhenCalled_ProperMethodCalled()
        {
            // Act
            await _sut.CompleteAsync();

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(new CancellationToken()));
        }

        [Fact]
        public void Dispose_WhenCalled_ProperMethodCalled()
        {
            // Act
            _sut.Dispose();

            // Assert
            _mockContext.Verify(c => c.Dispose());
        }
    }
}
