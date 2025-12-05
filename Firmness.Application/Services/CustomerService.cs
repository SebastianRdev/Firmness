namespace Firmness.Application.Services;

using System.Linq;
using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Firmness.Application.DTOs.Excel;

/// <summary>
/// Provides business logic operations for managing customers,
/// including creation, retrieval, updating, deletion, and searching.
/// </summary>

public class CustomerService : ICustomerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExcelService _excelService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService"/> class.
    /// </summary>
    /// <param name="customerRepository">The repository used for data access of customers.</param>
    /// <param name="mapper">The AutoMapper instance used for object mapping.</param>
    /// <param name="logger">The logger used to record application events and errors.</param>
    public CustomerService(
        UserManager<ApplicationUser> userManager,
        IExcelService excelService,
        IMapper mapper,
        ILogger<CustomerService> logger)
    {
        _userManager = userManager;
        _excelService = excelService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all customers.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing a collection of <see cref="CustomerDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
    public async Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var customerUsers = await GetCustomerUsersAsync();

            // Aplicamos la paginación a la lista completa
            var pagedUsers = customerUsers
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Mapeamos con roles incluidos
            var customerDtos = new List<CustomerDto>();

            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = _mapper.Map<CustomerDto>(user);
                dto.Roles = roles.ToList();
                customerDtos.Add(dto);
            }

            // If there is no data on that page
            if (!customerDtos.Any())
                return ResultOft<IEnumerable<CustomerDto>>.Failure("No customers found on this page.");

            return ResultOft<IEnumerable<CustomerDto>>.Success(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated customers.");
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error loading paginated customers.");
        }
    }


    public async Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ResultOft<CustomerDto>.Failure("The customer ID must be a valid GUID");

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Customer"))
                return ResultOft<CustomerDto>.Failure($"Customer with ID {id} not found");

            var dto = _mapper.Map<CustomerDto>(user);
            dto.Roles = roles.ToList();

            return ResultOft<CustomerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining the customer {CustomerId}", id);
            return ResultOft<CustomerDto>.Failure("Error loading customer. Please try again.");
        }
    }
    
    public async Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto)
    {
        try
        {
            var customer = _mapper.Map<ApplicationUser>(createDto);
            
            var result = await _userManager.CreateAsync(customer, createDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }
            
            // Assign default role
            var roleResult = await _userManager.AddToRoleAsync(customer, "Customer");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return ResultOft<CustomerDto>.Failure(errors);
            }

            // Map user to DTO
            var dto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Customer '{CustomerName}' created with ID {CustomerId}", customer.UserName, customer.Id);

            return ResultOft<CustomerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return ResultOft<CustomerDto>.Failure("Error creating customer. Please try again.");
        }
    }

    public async Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto)
    {
        try
        {
            var customer = await _userManager.FindByIdAsync(updateDto.Id.ToString());
            if (customer == null)
            {
                _logger.LogWarning("Attempt to update non-existent customer with ID {CustomerId}", updateDto.Id);
                return ResultOft<CustomerDto>.Failure($"Customer with ID {updateDto.Id} not found");
            }

            _mapper.Map(updateDto, customer);

            // If a new password was provided, we changed it
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                var passwordResult = await _userManager.RemovePasswordAsync(customer);
                if (!passwordResult.Succeeded)
                {
                    return ResultOft<CustomerDto>.Failure("Error removing the old password.");
                }

                var newPasswordResult = await _userManager.AddPasswordAsync(customer, updateDto.NewPassword);
                if (!newPasswordResult.Succeeded)
                {
                    return ResultOft<CustomerDto>.Failure("Error setting the new password.");
                }
            }

            await _userManager.UpdateAsync(customer);

            // We map the updated client to DTO to return it
            var dto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Updated customer '{CustomerName}' (ID: {CustomerId})", customer.UserName, customer.Id);

            return ResultOft<CustomerDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer update failed {CustomerId}", updateDto.Id);
            return ResultOft<CustomerDto>.Failure("Customer update failed. Please try again.");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result.Failure("The customer ID must be a valid GUID");
            }

            var customer = await _userManager.FindByIdAsync(id.ToString());
            if (customer == null)
            {
                _logger.LogWarning("Attempt to delete non-existent customer with ID {CustomerId}", id);
                return Result.Failure($"Customer with ID {id} not found");
            }

            var result = await _userManager.DeleteAsync(customer);

            if (!result.Succeeded)
            {
                return Result.Failure("Error deleting customer. Please try again.");
            }

            _logger.LogInformation("Customer with ID {CustomerId} removed", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting the customer {CustomerId}", id);
            return Result.Failure("Error deleting the customer. Please try again.");
        }
    }


    public async Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return ResultOft<IEnumerable<CustomerDto>>.Failure("The search term must be at least 2 characters long");
            }

            // Get all customers (users) from AspNetUsers
            var customers = await _userManager.Users.ToListAsync();
        
            var filtered = customers.Where(p => 
                    p.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) // Optional: Search by FullName or other fields
            );

            var dtos = _mapper.Map<IEnumerable<CustomerDto>>(filtered);
            _logger.LogInformation("Searching for customers with the term '{SearchTerm}' returned {Count} results", searchTerm, dtos.Count());
            return ResultOft<IEnumerable<CustomerDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for customers with term '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<CustomerDto>>.Failure("Error searching for customers. Please try again.");
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            // Check if the user exists in AspNetUsers
            var customer = await _userManager.FindByIdAsync(id.ToString());
            return customer != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if customer exists {CustomerId}", id);
            return false;
        }
    }
    
    // Obtener roles de un usuario específico
    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new Exception("User not found");
        return await _userManager.GetRolesAsync(user);
    }

    // Obtener todos los roles disponibles
    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        return new List<string> { "Admin", "Customer" };  // Roles predeterminados o puedes obtenerlos dinámicamente si es necesario.
    }

    // Actualizar el rol de un usuario
    public async Task UpdateUserRoleAsync(Guid userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new Exception("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray()); // Elimina los roles actuales
        await _userManager.AddToRoleAsync(user, newRole); // Asigna el nuevo rol
    }
    
    private async Task<List<ApplicationUser>> GetCustomerUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var customers = new List<ApplicationUser>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Customer"))
                customers.Add(user);
        }

        return customers;
    }

    public async Task<Result> ImportFromExcelAsync(IFormFile file, string entityType)
    {
        try
        {
            using var package = new ExcelPackage(file.OpenReadStream());
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;
            int columnCount = worksheet.Dimension.Columns;

            // 1. Leer encabezados originales
            var headers = new List<string>();
            for (int col = 1; col <= columnCount; col++)
            {
                headers.Add(worksheet.Cells[1, col].Text.Trim());
            }

            // 2. Corregir encabezados con IA
            var correction = await _excelService.CorrectColumnNamesAsync(
                headers,
                ColumnTemplates.Templates[entityType]
            );

            if (!correction.IsSuccess)
                return Result.Failure("Error correcting headers.");

            var corrected = correction.Data.CorrectedColumns;

            if (correction.Data.WasCorrected)
                _logger.LogInformation("Header corrections: {Report}", correction.Data.ChangesReport);

            // 3. Crear un diccionario header → índice
            var headerIndex = corrected
                .Select((value, index) => new { value, index })
                .ToDictionary(x => x.value.ToLower(), x => x.index + 1);

            // 4. Contadores
            int successCount = 0;
            var errors = new List<string>();

            // 5. Procesar filas
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // Crear DTO
                    var dto = new CreateCustomerDto
                    {
                        UserName = worksheet.Cells[row, headerIndex["username"]].Text,
                        FullName = worksheet.Cells[row, headerIndex["fullname"]].Text,
                        Email = worksheet.Cells[row, headerIndex["email"]].Text,
                        Address = worksheet.Cells[row, headerIndex["address"]].Text,
                        PhoneNumber = worksheet.Cells[row, headerIndex["phone"]].Text,

                        // Password temporal (buena práctica: usuarios cambian luego)
                        Password = "Temp123$"
                    };

                    // Validación mínima
                    if (string.IsNullOrWhiteSpace(dto.Email))
                    {
                        errors.Add($"Fila {row}: email vacío.");
                        continue;
                    }

                    // 6. Crear usuario con tu propio servicio
                    var createResult = await CreateAsync(dto);

                    if (!createResult.IsSuccess)
                    {
                        errors.Add($"Fila {row}: {createResult.ErrorMessage}");
                        continue;
                    }

                    successCount++;
                }
                catch (Exception exRow)
                {
                    errors.Add($"Fila {row}: error inesperado: {exRow.Message}");
                }
            }

            _logger.LogInformation("Excel import completed: {Success} OK, {Errors} errors", successCount, errors.Count);

            if (errors.Any())
                return Result.Failure($"Imported {successCount} customers with {errors.Count} errors:\n" +
                                    string.Join("\n", errors));

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel");
            return Result.Failure("Error processing the Excel file.");
        }
    }


    
    public async Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return ResultOft<ExcelHeadersResponseDto>.Failure("No file uploaded.");

            var headers = await _excelService.GetHeadersAsync(file);

            if (headers == null || headers.Count == 0)
                return ResultOft<ExcelHeadersResponseDto>.Failure("Excel contains no readable headers.");

            var dto = new ExcelHeadersResponseDto
            {
                OriginalHeaders = headers
            };

            return ResultOft<ExcelHeadersResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting headers.");
            return ResultOft<ExcelHeadersResponseDto>.Failure("Error processing the Excel file.");
        }
    }
    
}