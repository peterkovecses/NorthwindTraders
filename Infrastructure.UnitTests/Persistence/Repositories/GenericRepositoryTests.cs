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
        private readonly TestGenericRepository _sut;
        private readonly Expression<Func<TestClass, bool>> _truePredicate = _ => true;
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
            _sut = new TestGenericRepository(_contextMock.Object);
        }

        [Fact]
        public async Task Get_WhenNoPaginationGiven_AllEntriesAreReturned()
        {
            // Arrange                        

            // Act
            var (totalItems, items) = await _sut.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), _truePredicate);

            //Assert
            items.Should().BeEquivalentTo(TestEntities);
            totalItems.Should().Be(TestEntities.Count);
        }

        [Fact]
        public async Task Get_WhenPaginationGiven_PaginatedEntriesAreReturned()
        {
            // Arrange                        
            var pagination = new Pagination { PageNumber = 2, PageSize = 1 };
            var expected = TestEntities.Skip(1).Take(1);

            // Act
            var items = (await _sut.GetAsync(pagination, Sorting.NoSorting(), _truePredicate)).items;

            //Assert
            items.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_WhenNoSortingGiven_EntriesAreReturnedInTheOriginalOrder()
        {
            // Arrange                        

            // Act
            var items = (await _sut.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), _truePredicate)).items;

            //Assert
            items.First().Should().Be(TestEntities.First());
            items.Last().Should().Be(TestEntities.Last());
        }

        [Fact]
        public async Task Get_WhenSortingGivenWithAscendingOrder_EntriesAreReturnedInTheExpectedOrder()
        {
            // Arrange                        
            var sorting = new Sorting { SortBy = "Id" };
            var orderedEntities = TestEntities.OrderBy(x => x.Id);

            // Act
            var items = (await _sut.GetAsync(Pagination.NoPagination(), sorting, _truePredicate)).items;

            //Assert
            items.First().Should().Be(orderedEntities.First());
            items.Last().Should().Be(orderedEntities.Last());
        }

        [Fact]
        public async Task Get_WhenSortingGivenWithDescendingOrder_EntriesAreReturnedInTheExpectedOrder()
        {
            // Arrange                        
            var sorting = new Sorting { SortBy = "Id", DescendingOrder = true };
            var orderedEntities = TestEntities.OrderByDescending(x => x.Id);

            // Act
            var items = (await _sut.GetAsync(Pagination.NoPagination(), sorting, _truePredicate)).items;

            //Assert
            items.First().Should().Be(orderedEntities.First());
            items.Last().Should().Be(orderedEntities.Last());
        }

        [Fact]
        public async Task Get_WhenPredicateIsGiven_TheExpectedEntriesAreReturned()
        {
            // Arrange
            Expression<Func<TestClass, bool>> predicate = t => t.Id < 3;
            var expectedItems = TestEntities.Where(predicate.Compile());
            var expectedTotalItems = expectedItems.Count();

            // Act
            var (totalItems, items) = (await _sut.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), predicate));

            //Assert
            totalItems.Should().Be(expectedTotalItems);
            items.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task FindById_WhenValidIdPassed_EntryIsReturnedWithTheSameId()
        {
            // Arrange
            var id = 3;
            var expected = _dbSetMock.Object.Where(e => e.Id == id).First();
            _dbSetMock.Setup(d => d.FindAsync(It.IsAny<object[]>(), CancellationToken.None)).ReturnsAsync(expected);

            // Act
            var actual = await _sut.FindByIdAsync(id);

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task FindById_WhenInvalidIdPassed_ReturnsNull()
        {
            // Arrange
            var id = 6;
            var expected = _dbSetMock.Object.Where(e => e.Id == id).FirstOrDefault();
            _dbSetMock.Setup(d => d.FindAsync(It.IsAny<object[]>(), CancellationToken.None)).ReturnsAsync(expected);

            // Act
            var actual = await _sut.FindByIdAsync(id);

            //Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async Task Add_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var objectToAdd = new TestClass { Id = 10 };

            // Act
            await _sut.AddAsync(objectToAdd);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().AddAsync(objectToAdd, new CancellationToken()));
        }

        [Fact]
        public void Remove_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();
            var objectToRemove = new TestClass { Id = 1 };

            // Act
            _sut.Remove(objectToRemove);

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
