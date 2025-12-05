namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Customers;

/// <summary>
/// Controller for managing customers in the Web Admin interface.
/// </summary>
[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomersController"/> class.
    /// </summary>
    /// <param name="customerApiClient">The customer API client.</param>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="logger">The logger instance.</param>
    public CustomersController(
        ICustomerApiClient customerApiClient,
        IMapper mapper,
        ILogger<CustomersController> logger)
    {
        _customerApiClient = customerApiClient;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Displays a paginated list of customers.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The index view with the list of customers.</returns>
    // GET: /Customers?page=1
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await _customerApiClient.GetAllPaginatedAsync(page, pageSize);
    
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<CustomerViewModel>());
        }

        var viewModels = _mapper.Map<List<CustomerViewModel>>(result.Data);

        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;

        return View(viewModels);
    }

    /// <summary>
    /// Displays the view for creating a new customer.
    /// </summary>
    /// <returns>The create view.</returns>
    // GET: /Customers/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles the creation of a new customer.
    /// </summary>
    /// <param name="model">The create customer view model.</param>
    /// <returns>Redirects to the index view on success, or returns the create view with errors.</returns>
    // POST: /Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var createDto = _mapper.Map<CreateCustomerDto>(model);
        var result = await _customerApiClient.CreateAsync(createDto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["Success"] = $"Customer '{result.Data.UserName}' created successfully";
        return RedirectToAction(nameof(Index));
    }
    
    /// <summary>
    /// Displays the view for editing an existing customer.
    /// </summary>
    /// <param name="id">The ID of the customer to edit.</param>
    /// <returns>The edit view with the customer data.</returns>
    // GET: /Customers/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _customerApiClient.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var viewModel = _mapper.Map<EditCustomerViewModel>(result.Data);
        return View(viewModel);
    }

    /// <summary>
    /// Handles the update of an existing customer.
    /// </summary>
    /// <param name="id">The ID of the customer to update.</param>
    /// <param name="model">The edit customer view model.</param>
    /// <returns>Redirects to the index view on success, or returns the edit view with errors.</returns>
    // POST: /Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updateDto = _mapper.Map<UpdateCustomerDto>(model);
        var result = await _customerApiClient.UpdateAsync(updateDto);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(model);
        }

        TempData["Success"] = $"Customer '{result.Data.UserName}' successfully updated";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Retrieves customer details for deletion confirmation via AJAX.
    /// </summary>
    /// <param name="id">The ID of the customer to delete.</param>
    /// <returns>A JSON result containing the customer data or an error message.</returns>
    // GET: /Customers/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _customerApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }

        return Json(new { success = true, customer = result.Data });
    }

    /// <summary>
    /// Confirms the deletion of a customer.
    /// </summary>
    /// <param name="id">The ID of the customer to delete.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    // POST: /Customers/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _customerApiClient.DeleteAsync(id);
    
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }
    
        return Json(new { success = true });
    }

    /// <summary>
    /// Searches for customers based on a search term.
    /// </summary>
    /// <param name="term">The search term.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>The index view with the search results.</returns>
    // GET: /Customers/Search?term=cemento&page=1
    public async Task<IActionResult> Search(string term, int page = 1)
    {
        var result = await _customerApiClient.SearchAsync(term);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        ViewData["SearchTerm"] = term;
        ViewData["CurrentPage"] = page;
        return View("Index", _mapper.Map<List<CustomerViewModel>>(result.Data));
    }
    
    /// <summary>
    /// Handles the Excel file upload and extracts headers for import.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="entityType">The type of entity to import.</param>
    /// <returns>A JSON result containing extracted headers.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file, string entityType)
    {
        _logger.LogInformation("ImportExcel called with file: {FileName}, entityType: {EntityType}", 
            file?.FileName ?? "null", entityType ?? "null");

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("No file selected for import");
            return Json(new { success = false, message = "No file selected." });
        }

        if (string.IsNullOrEmpty(entityType))
        {
            _logger.LogWarning("Entity type not provided");
            return Json(new { success = false, message = "Entity type not provided." });
        }

        try
        {
            _logger.LogInformation("Calling API to extract headers...");
            
            var result = await _customerApiClient.ExtractHeadersFromExcelAsync(file, entityType);

            _logger.LogInformation("API Response - IsSuccess: {IsSuccess}, Data: {HasData}", 
                result.IsSuccess, result.Data != null);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("API returned error: {Error}", result.ErrorMessage);
                return Json(new { success = false, message = result.ErrorMessage ?? "Unknown error" });
            }

            if (result.Data == null)
            {
                _logger.LogWarning("API returned success but Data is null");
                return Json(new { success = false, message = "No data returned from API" });
            }

            if (result.Data.OriginalHeaders == null)
            {
                _logger.LogWarning("API returned data but OriginalHeaders is null");
                return Json(new { success = false, message = "Headers property is null" });
            }

            _logger.LogInformation("Successfully extracted {Count} headers: {Headers}", 
                result.Data.OriginalHeaders.Count, 
                string.Join(", ", result.Data.OriginalHeaders));

            return Json(new
            {
                success = true,
                headers = result.Data.OriginalHeaders,
                correctHeaders = result.Data.CorrectHeaders,
                entityType = entityType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in ImportExcel");
            return Json(new { success = false, message = $"Exception: {ex.Message}" });
        }
    }

    /// <summary>
    /// Uses AI to correct column headers based on a template.
    /// </summary>
    /// <param name="request">The request containing original and correct headers.</param>
    /// <returns>A JSON result with corrected headers.</returns>
    [HttpPost]
    public async Task<IActionResult> CorrectHeadersWithAI([FromBody] CorrectHeadersRequest request)
    {
        _logger.LogInformation("CorrectHeadersWithAI called with {OriginalCount} original and {CorrectCount} correct headers",
            request.OriginalHeaders?.Count ?? 0, request.CorrectHeaders?.Count ?? 0);

        if (request.OriginalHeaders == null || !request.OriginalHeaders.Any())
        {
            return Json(new { success = false, message = "Original headers are required" });
        }

        if (request.CorrectHeaders == null || !request.CorrectHeaders.Any())
        {
            return Json(new { success = false, message = "Correct headers are required" });
        }

        try
        {
            _logger.LogInformation("Calling API to correct headers with AI...");

            var result = await _customerApiClient.CorrectHeadersAsync(request.OriginalHeaders, request.CorrectHeaders);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("API returned error: {Error}", result.ErrorMessage);
                return Json(new { success = false, message = result.ErrorMessage ?? "Unknown error" });
            }

            if (result.Data == null)
            {
                _logger.LogWarning("API returned success but Data is null");
                return Json(new { success = false, message = "No data returned from API" });
            }

            _logger.LogInformation("AI correction completed. WasCorrected: {WasCorrected}", 
                result.Data.WasCorrected);

            return Json(new
            {
                success = true,
                data = new
                {
                    correctedColumns = result.Data.CorrectedColumns,
                    wasCorrected = result.Data.WasCorrected,
                    changesReport = result.Data.ChangesReport
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CorrectHeadersWithAI");
            return Json(new { success = false, message = $"Exception: {ex.Message}" });
        }
    }

    /// <summary>
    /// Processes the bulk insert of data using corrected headers.
    /// </summary>
    /// <param name="file">The Excel file containing data.</param>
    /// <param name="entityType">The type of entity to insert.</param>
    /// <param name="correctedHeaders">The list of corrected headers.</param>
    /// <returns>A JSON result with the number of inserted and failed rows.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessBulkInsert(IFormFile file, string entityType, List<string> correctedHeaders)
    {
        _logger.LogInformation("ProcessBulkInsert called. File: {File}, Entity: {Entity}", file?.FileName, entityType);

        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file uploaded" });

        if (string.IsNullOrWhiteSpace(entityType))
            return Json(new { success = false, message = "Entity type is required" });

        if (correctedHeaders == null || correctedHeaders.Count == 0)
            return Json(new { success = false, message = "Corrected headers are required" });

        try
        {
            var result = await _customerApiClient.BulkInsertAsync(file, entityType, correctedHeaders);

            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }

            return Json(new
            {
                success = true,
                inserted = result.Data.InsertedCount,
                failedRows = result.Data.FailedRows
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in ProcessBulkInsert");
            return Json(new { success = false, message = ex.Message });
        }
    }


    public class CorrectHeadersRequest
    {
        public List<string> OriginalHeaders { get; set; } = new();
        public List<string> CorrectHeaders { get; set; } = new();
    }
}