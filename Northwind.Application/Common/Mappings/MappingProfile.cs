using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Domain.Entities;

namespace Northwind.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
