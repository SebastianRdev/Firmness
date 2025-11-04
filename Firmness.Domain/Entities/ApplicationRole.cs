namespace Firmness.Domain.Entities;

using Microsoft.AspNetCore.Identity;

// Hereda de IdentityRole y usa string como tipo de clave (por defecto)
public class ApplicationRole : IdentityRole
{
    // Campo opcional: puedes agregar propiedades personalizadas si las necesitas
    public string? Description { get; set; }

    // Constructor vacío (necesario para EF)
    public ApplicationRole() : base() { }

    // Constructor que permite inicializar el rol directamente con nombre y descripción
    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}