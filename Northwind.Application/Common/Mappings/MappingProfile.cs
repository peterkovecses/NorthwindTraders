using AutoMapper;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CustomerDemographic, CustomerDemographicDto>();
            CreateMap<CustomerDemographicDto, CustomerDemographic>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailDto, OrderDetail>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<Region, RegionDto>();
            CreateMap<RegionDto, Region>();
            CreateMap<Shipper, ShipperDto>();
            CreateMap<ShipperDto, Shipper>();
            CreateMap<Supplier, SupplierDto>();
            CreateMap<SupplierDto, Supplier>();
            CreateMap<Territory, TerritoryDto>();
            CreateMap<TerritoryDto, Territory>();

            CreateMap<PaginationQuery, PaginationFilter>();
        }
    }
}
