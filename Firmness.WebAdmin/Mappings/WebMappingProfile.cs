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
        CreateMap<CategoryDto, CategoryViewModel>();
    }
}
