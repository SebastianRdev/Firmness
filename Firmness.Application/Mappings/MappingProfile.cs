namespace Firmness.Application.Mappings;

using AutoMapper;
using Firmness.Application.DTOs.Products;
using Firmness.Application.DTOs.Categories;
using Firmness.Domain.Entities;

/// <summary>
/// Configuration of mappings between domain entities and DTOs.
/// AutoMapper uses this class to learn how to convert objects.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product → ProductDto (to show)
        CreateMap<Product, ProductDto>();
        
        // Category → CategoryDto (to show)
        CreateMap<Category, CategoryDto>();
        
        // CreateProductDto → Product (to create)
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id lo genera la BD
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Lo asigna el servicio
            .ForMember(dest => dest.IsActive, opt => opt.Ignore()); // Lo asigna el servicio
        
        // CreateCategoryDto → Category (to create)
        CreateMap<CreateCategoryDto, Category>();
        
        // UpdateProductDto → Product (to update)
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<ProductDto, UpdateProductDto>();
        
        // CreateCategoryDto → Category (to update)
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<CategoryDto, UpdateCategoryDto>();
    }
}