using Microsoft.EntityFrameworkCore;
using Northwind.Infrastructure.Persistence;

namespace Infrastructure.UnitTests.Persistence
{
    public class UnitOfWorkTests
    {
        private readonly Mock<NorthwindContext> _contextMock;
        private readonly UnitOfWork _sut;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptions<NorthwindContext>();
            _contextMock = new Mock<NorthwindContext>(options);

            _sut = new UnitOfWork(_contextMock.Object);
        }

        [Fact]
        public async Task Complete_WhenCalled_ProperMethodCalled()
        {
            // Act
            await _sut.CompleteAsync();

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(new CancellationToken()));
        }

        [Fact]
        public void Dispose_WhenCalled_ProperMethodCalled()
        {
            // Act
            _sut.Dispose();

            // Assert
            _contextMock.Verify(c => c.Dispose());
        }
    }
}
