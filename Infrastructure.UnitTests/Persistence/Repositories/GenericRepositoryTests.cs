using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Northwind.Infrastructure.Persistence;
using Northwind.Infrastructure.Persistence.Repositories;
using System.Linq.Expressions;

namespace Infrastructure.UnitTests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly List<TestClass> TestEntities;

        public GenericRepositoryTests()
        {
            TestEntities = new List<TestClass>
            {
                new TestClass { Id = 1 },
                new TestClass { Id = 2 }
            };
        }

        [Fact]
        public async Task Get_WhenCalled_EntriesAreReturned()
        {
            // Arrange            
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

            // Act
            var result = await sut.GetAsync();

            //Assert
            result.Should().BeEquivalentTo(TestEntities);
        }

        [Fact]
        public async Task GetById_WhenValidIdPassed_EntryIsReturnedWithTheSameId()
        {
            // Arrange
            var testObject = new TestClass { Id = 3 };
            TestEntities.Add(testObject);

            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();            
            mockDBSet.Setup(d => d.FindAsync(testObject.Id)).ReturnsAsync(testObject);

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

            // Act
            var result = await sut.GetByIdAsync(testObject.Id);

            //Assert
            result.Should().BeEquivalentTo(testObject);
        }

        [Fact]
        public async Task GetById_WhenInvalidIdPassed_ReturnsNull()
        {
            // Arrange
            var id = 3;
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

            // Act
            var result = await sut.GetByIdAsync(id);

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
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

            var expected = new List<TestClass>
            {
                new TestClass { Id = 1 },
                new TestClass { Id = 2 }
            };

            // Act
            var result = await sut.FindAsync(predicate);

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
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

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
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

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
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);

            // Act & Assert
            sut.Invoking(x => x.FindSingleOrDefaultAsync(predicate)).Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Add_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);
            var objectToAdd = new TestClass();

            // Act
            await sut.AddAsync(objectToAdd);

            //Assert
            mockContext.Verify(x => x.Set<TestClass>().AddAsync(objectToAdd, new CancellationToken()));
        }

        [Fact]
        public async Task AddRange_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);
            var objectsToAdd = new List<TestClass> { new TestClass(), new TestClass() };

            // Act
            await sut.AddRangeAsync(objectsToAdd);

            //Assert
            mockContext.Verify(x => x.Set<TestClass>().AddRangeAsync(objectsToAdd, new CancellationToken()));
        }

        [Fact]
        public void Remove_WhenObjectPassed_ProperMethodCalled()
        {
            // Arrange            
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);
            var objectToRemove = new TestClass { Id = 1 };

            // Act
            sut.Remove(objectToRemove);

            //Assert
            mockContext.Verify(x => x.Set<TestClass>().Remove(objectToRemove));
        }

        [Fact]
        public void RemoveRange_WhenObjectsPassed_ProperMethodCalled()
        {
            // Arrange            
            var mockDBSet = TestEntities.AsQueryable().BuildMockDbSet();

            var mockContext = GetMockContext();
            mockContext.Setup(c => c.Set<TestClass>()).Returns(mockDBSet.Object);

            var sut = new GenericRepository<TestClass>(mockContext.Object);
            var objectsToRemove = new List<TestClass> { new TestClass { Id = 1 }, new TestClass { Id = 2 } };

            // Act
            sut.RemoveRange(objectsToRemove);

            //Assert
            mockContext.Verify(x => x.Set<TestClass>().RemoveRange(objectsToRemove));
        }

        private static Mock<DbContext> GetMockContext()
        {
            var options = new DbContextOptions<DbContext>();
            var context = new Mock<DbContext>(options);
            return context;
        }
    }

    public class TestClass
    {
        public int Id { get; set; }
    }
}
