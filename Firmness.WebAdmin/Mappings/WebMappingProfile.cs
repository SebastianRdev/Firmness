namespace Firmness.WebAdmin.Mappings;

using AutoMapper;
using Firmness.Application.DTOs.Products;
using Firmness.WebAdmin.Models;
using Firmness.Application.DTOs.Categories;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<ProductDto, ProductViewModel>();
        CreateMap<ProductDto, EditProductViewModel>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<CategoryDto, CategoryViewModel>();
        CreateMap<CategoryDto, EditCategoryViewModel>();

        CreateMap<EditProductViewModel, UpdateProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
    }
}
