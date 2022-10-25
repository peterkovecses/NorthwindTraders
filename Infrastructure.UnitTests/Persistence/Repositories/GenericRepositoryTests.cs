﻿using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Northwind.Application.Extensions;
using Northwind.Application.Models;
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

        private readonly Mock<DbSet<TestClass>> _dbSetMock;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptions<DbContext>();
            _contextMock = new Mock<DbContext>(options);
            _dbSetMock = TestEntities.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Set<TestClass>()).Returns(_dbSetMock.Object);
        }

        [Fact]
        public async Task Get_WhenNoPaginationParameterGiven_ProperMethodsCalled()
        {
            // Arrange                        
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(new Pagination());

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
        }

        [Fact]
        public async Task Get_WhenPaginationGiven_ProperMethodsCalled()
        {
            // Arrange            
            var pagination = new Pagination { PageNumber = 2, PageSize = 3 };
            var query = _contextMock.Object.Set<TestClass>().AsQueryable();
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(pagination);

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
        }

        [Fact]
        public async Task Get_WhenSortingGiven_ProperMethodsCalled()
        {
            // Arrange            
            var sorting = new Sorting { SortBy = "Id", DescendingOrder = true };
            var query = _contextMock.Object.Set<TestClass>().AsQueryable().OrderByCustom(sorting);
            var sut = new TestGenericRepository(_contextMock.Object);

            // Act
            var result = await sut.GetAsync(new Pagination(), sorting: sorting);

            //Assert
            _contextMock.Verify(c => c.Set<TestClass>());
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
            var result = await sut.GetAsync(new Pagination(), predicate: predicate);

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
    }

    public class TestGenericRepository : GenericRepository<TestClass, int>
    {
        public TestGenericRepository(DbContext context) : base(context)
        {
        }
    }
}
