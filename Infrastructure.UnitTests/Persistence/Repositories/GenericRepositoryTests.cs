using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Northwind.Application.Models;
using Northwind.Infrastructure.Persistence.Repositories;
using System.Linq.Expressions;

namespace Infrastructure.UnitTests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly Mock<DbContext> _contextMock;
        private readonly Mock<DbSet<TestClass>> _dbSetMock;
        private readonly List<TestClass> TestEntities = new()
        {
                new TestClass { Id = 2 },
                new TestClass { Id = 1 },
                new TestClass { Id = 3 },
                new TestClass { Id = 5 },
                new TestClass { Id = 4 }
            };

        public GenericRepositoryTests()
        {
            var options = new DbContextOptions<DbContext>();
            _contextMock = new Mock<DbContext>(options);
            _dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Set<TestClass>()).Returns(_dbSetMock.Object);
        }

        [Fact]
        public async Task Get_WhenNoPaginationGiven_AllEntriesAreReturned()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(Pagination.NoPagination, Sorting.NoSorting);

            //Assert
            result.items.Should().BeEquivalentTo(TestEntities);
            result.totalItems.Should().Be(TestEntities.Count);
        }

        [Fact]
        public async Task Get_WhenPaginationGiven_PaginatedEntriesAreReturned()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);
            var pagination = new Pagination { PageNumber = 2, PageSize = 1 };
            var expected = TestEntities.Skip(1).Take(1);

            // Act
            var result = await sut.GetAsync(pagination, Sorting.NoSorting);

            //Assert
            result.items.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_WhenNoSortingGiven_EntriesAreReturnedInTheOriginalOrder()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(Pagination.NoPagination, Sorting.NoSorting);

            //Assert
            result.items.First().Should().Be(TestEntities.First());
            result.items.Last().Should().Be(TestEntities.Last());
        }

        [Fact]
        public async Task Get_WhenSortingGivenWithAscendingOrder_EntriesAreReturnedInTheExpectedOrder()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);
            var sorting = new Sorting { SortBy = "Id" };
            var orderedEntities = TestEntities.OrderBy(x => x.Id);

            // Act
            var result = await sut.GetAsync(Pagination.NoPagination, sorting);

            //Assert
            result.items.First().Should().Be(orderedEntities.First());
            result.items.Last().Should().Be(orderedEntities.Last());
        }

        [Fact]
        public async Task Get_WhenSortingGivenWithDescendingOrder_EntriesAreReturnedInTheExpectedOrder()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);
            var sorting = new Sorting { SortBy = "Id", DescendingOrder = true };
            var orderedEntities = TestEntities.OrderByDescending(x => x.Id);

            // Act
            var result = await sut.GetAsync(Pagination.NoPagination, sorting);

            //Assert
            result.items.First().Should().Be(orderedEntities.First());
            result.items.Last().Should().Be(orderedEntities.Last());
        }

        [Fact]
        public async Task Get_WhenPredicateIsGiven_TheExpectedEntriesAreReturned()
        {
            // Arrange
            Expression<Func<TestClass, bool>> predicate = t => t.Id < 3;
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            query = query.Where(predicate);
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(Pagination.NoPagination, Sorting.NoSorting, predicate);

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
        }

        [Fact]
        public async Task FindById_WhenValidIdPassed_EntryIsReturnedWithTheSameId()
        {
            // Arrange
            var testObject = new TestClass { Id = 6 };
            TestEntities.Add(testObject);
            _dbSetMock.Setup(d => d.FindAsync(new object?[] { testObject.Id }, new CancellationToken())).ReturnsAsync(testObject);
            var sut = new TestGenericRepository(_contextMock.Object);

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
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.FindByIdAsync(id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Add_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var sut = new TestGenericRepository(_contextMock.Object);
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
            var sut = new TestGenericRepository(_contextMock.Object);
            var objectToRemove = new TestClass { Id = 1 };

            // Act
            sut.Remove(objectToRemove);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().Remove(objectToRemove));
        }
    }

    public class TestClass
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }

    public class TestGenericRepository : GenericRepository<TestClass, int>
    {
        public TestGenericRepository(DbContext context) : base(context)
        {
        }
    }
}
