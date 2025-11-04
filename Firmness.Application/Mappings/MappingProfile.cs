namespace Firmness.Application.Mappings;

using AutoMapper;
using Firmness.Application.DTOs.Products;
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
        
        // CreateProductDto → Product (to create)
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id lo genera la BD
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Lo asigna el servicio
            .ForMember(dest => dest.IsActive, opt => opt.Ignore()); // Lo asigna el servicio
        
        // UpdateProductDto → Product (to update)
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<ProductDto, UpdateProductDto>();
    }
}