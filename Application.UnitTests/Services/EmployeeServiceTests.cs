using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Application.Services;
using Northwind.Domain.Entities;
using System.Linq.Expressions;

namespace Application.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPaginatedUriService> _uriServiceMock;
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _uriServiceMock = new Mock<IPaginatedUriService>();
            _sut = new EmployeeService(_unitOfWorkMock.Object, _mapperMock.Object, _uriServiceMock.Object);
        }

        [Fact]
        public async Task Get_WhenPaginationParameterPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeesMock = new Mock<IEnumerable<Employee>>();
            var totalEmployees = 10;
            var queryParameters = new QueryParameters<EmployeeFilter> { Pagination = new Pagination() };
            var next = "next";
            var previous = "previous";

            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(queryParameters.Pagination, null, null)).Returns(Task.FromResult((totalEmployees, employeesMock.Object)));
            _uriServiceMock.Setup(u => u.GetNavigations(queryParameters.Pagination)).Returns((next, previous));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAsync(queryParameters);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, null));
            _uriServiceMock.Verify(u => u.GetNavigations(queryParameters.Pagination));
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task FindById_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 12;
            var employee = new Employee { EmployeeId = id };
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(id)).Returns(Task.FromResult(employee));
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.FindByIdAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(id));
            _mapperMock.Verify(m => m.Map<EmployeeDto>(employee));
        }

        [Fact]
        public async Task Create_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto();
            var employee = new Employee();
            var id = 20;
            _unitOfWorkMock.Setup(u => u.Employees.AddAsync(employee)).Returns(Task.FromResult(id));
            _mapperMock.Setup(m => m.Map<Employee>(employeeDto)).Returns(employee);

            // Act
            await _sut.CreateAsync(employeeDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.AddAsync(employee));
            _mapperMock.Verify(m => m.Map<Employee>(employeeDto));
        }

        [Fact]
        public async Task Update_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto { EmployeeId = 30 };
            var employeeInDb = new Employee();
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(employeeDto.EmployeeId)).Returns(Task.FromResult(employeeInDb));
            _mapperMock.Setup(m => m.Map(employeeDto, employeeInDb));

            // Act
            await _sut.UpdateAsync(employeeDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(employeeDto.EmployeeId));
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
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>())).Returns(Task.FromResult((totalEmployees, employeesMock.Object)));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.DeleteAsync(ids);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>()));
            _unitOfWorkMock.Verify(u => u.Employees.Remove(employeesMock.Object));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task IsExists_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            _unitOfWorkMock.Setup(u => u.Employees.FindByIdAsync(id)).Returns(Task.FromResult(new Employee()));

            // Act
            await _sut.IsExists(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindByIdAsync(id));
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

            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>())).Returns(Task.FromResult<(int, IEnumerable<Employee>)>((employees.Count(), employees)));           

            // Act
            await _sut.AreExists(ids);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(null, null, It.IsAny<Expression<Func<Employee, bool>>>()));
        }
    }
}
