using AutoMapper;
using Firmness.Application.DTOs.Products;
using Firmness.Domain.Entities;

namespace Firmness.Application.Mappings;

/// <summary>
/// Configuración de mapeos entre entidades del dominio y DTOs.
/// AutoMapper usa esta clase para saber cómo convertir objetos.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product → ProductDto (para mostrar)
        CreateMap<Product, ProductDto>();
        
        // CreateProductDto → Product (para crear)
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id lo genera la BD
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Lo asigna el servicio
            .ForMember(dest => dest.IsActive, opt => opt.Ignore()); // Lo asigna el servicio
        
        // UpdateProductDto → Product (para actualizar)
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<ProductDto, UpdateProductDto>();
    }
}