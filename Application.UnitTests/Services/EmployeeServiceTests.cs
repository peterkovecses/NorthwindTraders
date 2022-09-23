using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Dtos;
using Northwind.Application.Services;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Application.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _sut = new EmployeeService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Get_WhenCalled_ProperMethodsCalled()
        {
            // Arrange
            var mockEmployees = new Mock<IEnumerable<Employee>>();
            _mockUnitOfWork.Setup(u => u.Employees.GetAsync()).Returns(Task.FromResult(mockEmployees.Object));
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(mockEmployees.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAsync();

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.GetAsync());
            _mockMapper.Verify(m => m.Map<IEnumerable<EmployeeDto>>(mockEmployees.Object));
        }

        [Fact]
        public async Task GetById_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 12;
            var employee = new Employee { EmployeeId = id };
            _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(id)).Returns(Task.FromResult(employee));
            _mockMapper.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.GetByIdAsync(id);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.GetByIdAsync(id));
            _mockMapper.Verify(m => m.Map<EmployeeDto>(employee));
        }

        [Fact]
        public async Task Create_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto();
            var employee = new Employee();
            var id = 20;
            _mockUnitOfWork.Setup(u => u.Employees.AddAsync(employee)).Returns(Task.FromResult(id));
            _mockMapper.Setup(m => m.Map<Employee>(employeeDto)).Returns(employee);

            // Act
            await _sut.CreateAsync(employeeDto);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.AddAsync(employee));
            _mockMapper.Verify(m => m.Map<Employee>(employeeDto));
        }

        [Fact]
        public async Task Update_WhenObjectPassed_ProperMethodsCalled()
        {
            // Arrange
            var employeeDto = new EmployeeDto { EmployeeId = 30 };
            var employeeInDb = new Employee();
            _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(employeeDto.EmployeeId)).Returns(Task.FromResult(employeeInDb));
            _mockMapper.Setup(m => m.Map(employeeDto, employeeInDb));

            // Act
            await _sut.UpdateAsync(employeeDto);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.GetByIdAsync(employeeDto.EmployeeId));
            _mockUnitOfWork.Verify(u => u.CompleteAsync());
            _mockMapper.Verify(m => m.Map(employeeDto, employeeInDb));
        }

        [Fact]
        public async Task Delete_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            var employee = new Employee { EmployeeId = id };
            _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(id)).Returns(Task.FromResult(employee));
            _mockMapper.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.DeleteAsync(id);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.GetByIdAsync(id));
            _mockUnitOfWork.Verify(u => u.Employees.Remove(employee));
            _mockUnitOfWork.Verify(u => u.CompleteAsync());
            _mockMapper.Verify(m => m.Map<EmployeeDto>(employee));
        }

        [Fact]
        public async Task DeleteRange_WhenIdsArePassed_ProperMethodsCalled()
        {
            // Arrange
            var ids = new int[] { 9, 12, 17 };
            var employees = new Mock<IEnumerable<Employee>>().Object;
            _mockUnitOfWork.Setup(u => u.Employees.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(Task.FromResult(employees));
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employees)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.DeleteRangeAsync(ids);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>()));
            _mockUnitOfWork.Verify(u => u.Employees.RemoveRange(employees));
            _mockUnitOfWork.Verify(u => u.CompleteAsync());
            _mockMapper.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [Fact]
        public async Task IsExists_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(id)).Returns(Task.FromResult(new Employee()));

            // Act
            await _sut.IsExists(id);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.GetByIdAsync(id));
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

            _mockUnitOfWork.Setup(u => u.Employees.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(Task.FromResult(employees));           

            // Act
            await _sut.AreExists(ids);

            // Assert
            _mockUnitOfWork.Verify(u => u.Employees.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>()));
        }
    }
}
