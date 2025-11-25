namespace Firmness.WebAdmin.Mappings;

using AutoMapper;
using Firmness.Application.DTOs.Products;
using Firmness.Application.DTOs.Categories;
using Firmness.Application.DTOs.Customers;
using Firmness.WebAdmin.Models.Products;
using Firmness.WebAdmin.Models.Categories;
using Firmness.WebAdmin.Models.Customers;
using Microsoft.AspNetCore.Mvc.Rendering;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<ProductDto, ProductViewModel>();
        CreateMap<ProductDto, EditProductViewModel>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<CategoryDto, CategoryViewModel>();
        CreateMap<CategoryDto, EditCategoryViewModel>();
        CreateMap<CustomerDto, CustomerViewModel>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(role => new SelectListItem
            {
                Text = role,
                Value = role
            }).ToList()));
        
        CreateMap<CreateCustomerViewModel, CreateCustomerDto>();

        CreateMap<CustomerDto, EditCustomerViewModel>();

        CreateMap<EditProductViewModel, UpdateProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<EditCategoryViewModel, UpdateCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<EditCustomerViewModel, UpdateCustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.NewPassword, opt => opt.MapFrom(src => src.NewPassword))
            .ForMember(dest => dest.ConfirmNewPassword, opt => opt.MapFrom(src => src.ConfirmNewPassword));
    }
}
