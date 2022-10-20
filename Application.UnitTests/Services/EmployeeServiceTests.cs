using AutoMapper;
using LinqKit;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Application.Services;
using Northwind.Application.Services.PredicateBuilders;
using Northwind.Domain.Entities;
using System.Linq.Expressions;

namespace Application.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<EmployeePredicateBuilder> _predicateBuilderMock;
        private readonly CancellationToken _token;
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _predicateBuilderMock = new Mock<EmployeePredicateBuilder>();
            _token = new CancellationToken();
            _sut = new EmployeeService(_unitOfWorkMock.Object, _mapperMock.Object, _predicateBuilderMock.Object);
        }

        [Fact]
        public async Task Get_WhenPaginationParameterPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeesMock = new Mock<IEnumerable<Employee>>();
            var totalEmployees = 10;
            var queryParameters = new QueryParameters<EmployeeFilter> { Pagination = new Pagination() };

            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(queryParameters.Pagination, null, null, _token)).Returns(Task.FromResult((totalEmployees, employeesMock.Object)));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, null, _token));
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task Get_WhenSortingParameterPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeesMock = new Mock<IEnumerable<Employee>>();
            var totalEmployees = 10;
            var queryParameters = new QueryParameters<EmployeeFilter> { Sorting = new Sorting { SortBy = "LastName" } };

            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, queryParameters.Sorting, null, _token)).Returns(Task.FromResult((totalEmployees, employeesMock.Object)));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, queryParameters.Sorting, null, _token));
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task Get_WhenFilterParameterPassed_ProperMethodsCalled()
        {
            // Arrange
            IEnumerable<Employee> employees = new List<Employee>();
            var queryParameters = new QueryParameters<EmployeeFilter> { Filter = new EmployeeFilter() };
            var predicate = PredicateBuilder.New<Employee>(true);

            _predicateBuilderMock.Setup(builder => builder.GetPredicate(queryParameters)).Returns(predicate);
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, null, predicate, _token)).Returns(Task.FromResult((1, employees)));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employees)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, null, predicate, _token));
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [Fact]
        public async Task FindById_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 12;
            var employee = new Employee { EmployeeId = id };
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(id, _token)).Returns(Task.FromResult(employee));
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.FindByIdAsync(id, _token);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(id, _token));
            _mapperMock.Verify(m => m.Map<EmployeeDto>(employee));
        }

        [Fact]
        public async Task Create_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto();
            var employee = new Employee();
            var id = 20;
            _unitOfWorkMock.Setup(u => u.Employees.AddAsync(employee, _token)).Returns(Task.FromResult(id));
            _mapperMock.Setup(m => m.Map<Employee>(employeeDto)).Returns(employee);

            // Act
            await _sut.CreateAsync(employeeDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.AddAsync(employee, _token));
            _mapperMock.Verify(m => m.Map<Employee>(employeeDto));
        }

        [Fact]
        public async Task Update_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto { EmployeeId = 30 };
            var employeeInDb = new Employee();
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(employeeDto.EmployeeId, _token)).Returns(Task.FromResult(employeeInDb));
            _mapperMock.Setup(m => m.Map(employeeDto, employeeInDb));

            // Act
            await _sut.UpdateAsync(employeeDto, _token);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(employeeDto.EmployeeId, _token));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map(employeeDto, employeeInDb));
        }

        [Fact]
        public async Task Delete_WhenIdsArePassed_ProperMethodsCalled()
        {
            // Arrange
            var ids = new int[] { 9, 12, 17 };
            var employeesMock = new Mock<IEnumerable<Employee>>();
            var totalEmployees = 10;
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>(), _token)).Returns(Task.FromResult((totalEmployees, employeesMock.Object)));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.DeleteAsync(ids, _token);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>(), _token));
            _unitOfWorkMock.Verify(u => u.Employees.Remove(employeesMock.Object));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task IsExists_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(id, _token)).Returns(Task.FromResult(new Employee()));

            // Act
            await _sut.IsExists(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(id, _token));
        }

        [Fact]
        public async Task AreExists_WhenIdsArePassed_ProperMethodCalled()
        {
            // Arrange
            var ids = new int[] { 9, 12, 17 };
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = ids[0] },
                new Employee { EmployeeId = ids[1] },
                new Employee { EmployeeId = ids[2] }
            }.AsEnumerable();

            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>(), _token)).Returns(Task.FromResult((employees.Count(), employees)));           

            // Act
            await _sut.AreExists(ids);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>(), _token));
        }
    }
}
