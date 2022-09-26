using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Northwind.Infrastructure.Persistence.Repositories;
using System.Linq.Expressions;

namespace Infrastructure.UnitTests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly Mock<DbContext> _contextMock;
        private readonly List<TestClass> TestEntities;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptions<DbContext>();
            _contextMock = new Mock<DbContext>(options);

            TestEntities = new List<TestClass>
            {
                new TestClass { Id = 1 },
                new TestClass { Id = 2 }
            };
        }

        [Fact]
        public async Task GetAll_WhenCalled_EntriesAreReturned()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAllAsync();

            //Assert
            result.Should().BeEquivalentTo(TestEntities);
        }

        [Fact]
        public async Task Get_WhenValidIdPassed_EntryIsReturnedWithTheSameId()
        {
            // Arrange
            var testObject = new TestClass { Id = 3 };
            TestEntities.Add(testObject);

            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();            
            dbSetMock.Setup(d => d.FindAsync(testObject.Id)).ReturnsAsync(testObject);

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(testObject.Id);

            //Assert
            result.Should().BeEquivalentTo(testObject);
        }

        [Fact]
        public async Task GetById_WhenInvalidIdPassed_ReturnsNull()
        {
            // Arrange
            var id = 3;
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Find_WhenPredicateIsPassed_TheExpectedEntriesAreReturned()
        {
            // Arrange
            var testObject = new TestClass { Id = 3 };
            TestEntities.Add(testObject);

            Expression<Func<TestClass, bool>> predicate = t => t.Id < 3;
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            var expected = new List<TestClass>
            {
                new TestClass { Id = 1 },
                new TestClass { Id = 2 }
            };

            // Act
            var result = await sut.FindAllAsync(predicate);

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task FindSingleOrDefault_WhenPredicateIsPassed_TheExpectedEntryIsReturned()
        {
            // Arrange
            var expected = new TestClass { Id = 3 };
            TestEntities.Add(expected);

            Expression<Func<TestClass, bool>> predicate = t => t.Id == 3;
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.FindSingleOrDefaultAsync(predicate);

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task FindSingleOrDefault_WhenNoEntryMatchesTheCondition_ReturnsNull()
        {
            // Arrange
            Expression<Func<TestClass, bool>> predicate = t => t.Id == 3;
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.FindSingleOrDefaultAsync(predicate);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindSingleOrDefault_WhenMultipleEntriesMatchTheCondition_ExceptionIsThrown()
        {
            // Arrange
            Expression<Func<TestClass, bool>> predicate = t => t.Id < 3;
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);

            // Act & Assert
            sut.Invoking(x => x.FindSingleOrDefaultAsync(predicate)).Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Add_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);
            var objectToAdd = new TestClass();

            // Act
            await sut.AddAsync(objectToAdd);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().AddAsync(objectToAdd, new CancellationToken()));
        }

        [Fact]
        public async Task AddRange_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);
            var objectsToAdd = new List<TestClass> { new TestClass(), new TestClass() };

            // Act
            await sut.AddRangeAsync(objectsToAdd);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().AddRangeAsync(objectsToAdd, new CancellationToken()));
        }

        [Fact]
        public void Remove_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);
            var objectToRemove = new TestClass { Id = 1 };

            // Act
            sut.Remove(objectToRemove);

            //Assert
            _contextMock.Verify(x => x.Set<TestClass>().Remove(objectToRemove));
        }

        [Fact]
        public void RemoveRange_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(c => c.Set<TestClass>()).Returns(dbSetMock.Object);

            var sut = new TestGenericRepository(_contextMock.Object);
            var objectsToRemove = new List<TestClass> { new TestClass { Id = 1 }, new TestClass { Id = 2 } };

            // Act
            sut.RemoveRange(objectsToRemove);

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
        public TestGenericRepository(DbContext context) : base(context)
        {
        }
    }
}
