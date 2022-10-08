using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure.Persistence.Repositories;
using System.Linq.Expressions;

namespace Infrastructure.UnitTests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly Mock<DbContext> _contextMock;
        private readonly List<TestClass> TestEntities = new List<TestClass>
            {
                new TestClass { Id = 1 },
                new TestClass { Id = 2 },
                new TestClass { Id = 3 },
                new TestClass { Id = 4 },
                new TestClass { Id = 5 }
            };

        private Mock<DbSet<TestClass>> _dbSetMock;
        private readonly Mock<IStrategyResolver> _strategyResolverMock;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptions<DbContext>();
            _contextMock = new Mock<DbContext>(options);
            _dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Set<TestClass>()).Returns(_dbSetMock.Object);
            _strategyResolverMock = new Mock<IStrategyResolver>();
        }

        [Fact]
        public async Task Get_WhenPaginationQueryParameterNotGiven_ProperMethodsCalled()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var noPaginationStrategyMock = new Mock<IPaginationStrategy<TestClass>>();
            _strategyResolverMock.Setup(s => s.GetStrategy(query, null)).Returns(noPaginationStrategyMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);

            // Act
            var result = await sut.GetAsync();

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
            _strategyResolverMock.Verify(s => s.GetStrategy(query, null));
        }

        [Fact]
        public async Task Get_WhenPaginationFilterParameterGiven_ProperMethodsCalled()
        {
            // Arrange            
            var paginationQueryMock = new Mock<IPaginationQuery>();
            paginationQueryMock.Setup(p => p.PageNumber).Returns(2);
            paginationQueryMock.Setup(p => p.PageSize).Returns(2);

            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var paginationStrategyMock = new Mock<IPaginationStrategy<TestClass>>();

            _strategyResolverMock.Setup(s => s.GetStrategy(query, paginationQueryMock.Object)).Returns(paginationStrategyMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);

            // Act
            var result = await sut.GetAsync(paginationQueryMock.Object);

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
            _strategyResolverMock.Verify(s => s.GetStrategy(query, paginationQueryMock.Object));
        }

        [Fact]
        public async Task Get_WhenPredicateIsGiven_TheExpectedEntriesAreReturned()
        {
            // Arrange
            Expression<Func<TestClass, bool>> predicate = t => t.Id < 3;
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            query = query.Where(predicate);

            var noPaginationStrategyMock = new Mock<IPaginationStrategy<TestClass>>();
            noPaginationStrategyMock.Setup(strategy => strategy.GetItemsAsync());
            _strategyResolverMock.Setup(s => s.GetStrategy(query, null)).Returns(noPaginationStrategyMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);

            // Act
            var result = await sut.GetAsync(predicate: predicate);

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
            _strategyResolverMock.Verify(s => s.GetStrategy(query, null));
        }

        [Fact]
        public async Task FindById_WhenValidIdPassed_EntryIsReturnedWithTheSameId()
        {
            // Arrange
            var testObject = new TestClass { Id = 3 };
            TestEntities.Add(testObject);
            _dbSetMock.Setup(d => d.FindAsync(testObject.Id)).ReturnsAsync(testObject);
            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);

            // Act
            var result = await sut.FindByIdAsync(testObject.Id);

            //Assert
            result.Should().BeEquivalentTo(testObject);
        }

        [Fact]
        public async Task FindById_WhenInvalidIdPassed_ReturnsNull()
        {
            // Arrange
            var id = 3;
            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);

            // Act
            var result = await sut.FindByIdAsync(id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Add_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);
            var objectToAdd = new TestClass();

            // Act
            await sut.AddAsync(objectToAdd);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().AddAsync(objectToAdd, new CancellationToken()));
        }

        [Fact]
        public void Remove_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();
            var sut = new TestGenericRepository(_contextMock.Object, _strategyResolverMock.Object);
            var objectsToRemove = new List<TestClass> { new TestClass { Id = 1 }, new TestClass { Id = 2 } };

            // Act
            sut.Remove(objectsToRemove);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().RemoveRange(objectsToRemove));
        }
    }

    public class TestClass
    {
        public int Id { get; set; }
    }

    public class TestGenericRepository : GenericRepository<TestClass, int>
    {
        public TestGenericRepository(DbContext context, IStrategyResolver strategyResolver) : base(context, strategyResolver)
        {
        }
    }
}
