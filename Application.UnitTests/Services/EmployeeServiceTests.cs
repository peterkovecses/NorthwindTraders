using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Dtos;
using Northwind.Application.Services;
using Northwind.Domain.Common.Interfaces;
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
        public async Task GetAll_WhenCalled_ProperMethodsCalled()
        {
            // Arrange
            var employeesMock = new Mock<IEnumerable<Employee>>();
            _unitOfWorkMock.Setup(u => u.Employees.GetAllAsync(null)).Returns(Task.FromResult(employeesMock.Object));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.GetAllAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAllAsync(null));
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock.Object));
        }

        [Fact]
        public async Task Get_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 12;
            var employee = new Employee { EmployeeId = id };
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(id)).Returns(Task.FromResult(employee));
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.GetAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(id));
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
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(employeeDto.EmployeeId)).Returns(Task.FromResult(employeeInDb));
            _mapperMock.Setup(m => m.Map(employeeDto, employeeInDb));

            // Act
            await _sut.UpdateAsync(employeeDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(employeeDto.EmployeeId));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map(employeeDto, employeeInDb));
        }

        [Fact]
        public async Task Delete_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            var employee = new Employee { EmployeeId = id };
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(id)).Returns(Task.FromResult(employee));
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(new EmployeeDto());

            // Act
            await _sut.DeleteAsync(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(id));
            _unitOfWorkMock.Verify(u => u.Employees.Remove(employee));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map<EmployeeDto>(employee));
        }

        [Fact]
        public async Task DeleteRange_WhenIdsArePassed_ProperMethodsCalled()
        {
            // Arrange
            var ids = new int[] { 9, 12, 17 };
            var employeesMock = new Mock<IEnumerable<Employee>>().Object;
            _unitOfWorkMock.Setup(u => u.Employees.FindAllAsync(It.IsAny<Expression<Func<Employee, bool>>>(), null)).Returns(Task.FromResult(employeesMock));
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock)).Returns(new List<EmployeeDto>());

            // Act
            await _sut.DeleteRangeAsync(ids);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindAllAsync(It.IsAny<Expression<Func<Employee, bool>>>(), null));
            _unitOfWorkMock.Verify(u => u.Employees.RemoveRange(employeesMock));
            _unitOfWorkMock.Verify(u => u.CompleteAsync());
            _mapperMock.Verify(m => m.Map<IEnumerable<EmployeeDto>>(employeesMock));
        }

        [Fact]
        public async Task IsExists_WhenIdPassed_ProperMethodsCalled()
        {
            // Arrange
            var id = 20;
            _unitOfWorkMock.Setup(u => u.Employees.GetAsync(id)).Returns(Task.FromResult(new Employee()));

            // Act
            await _sut.IsExists(id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.GetAsync(id));
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

            _unitOfWorkMock.Setup(u => u.Employees.FindAllAsync(It.IsAny<Expression<Func<Employee, bool>>>(), null)).Returns(Task.FromResult(employees));           

            // Act
            await _sut.AreExists(ids);

            // Assert
            _unitOfWorkMock.Verify(u => u.Employees.FindAllAsync(It.IsAny<Expression<Func<Employee, bool>>>(), null));
        }
    }
}
