using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure.Persistence;
using Northwind.Infrastructure.Persistence.Interceptors;

namespace Infrastructure.UnitTests.Persistence
{
    public class UnitOfWorkTests
    {
        private readonly Mock<NorthwindContext> _contextMock;
        private readonly UnitOfWork _sut;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptions<NorthwindContext>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var interceptorMock = new Mock<AuditInterceptor>(dateTimeProviderMock.Object);
            _contextMock = new Mock<NorthwindContext>(options, interceptorMock.Object);
            var strategyResolverMock = new Mock<IStrategyResolver>();

            _sut = new UnitOfWork(_contextMock.Object, strategyResolverMock.Object);
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
