namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Customers;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerApiClient customerApiClient,
        IMapper mapper,
        ILogger<CustomersController> logger)
    {
        _customerApiClient = customerApiClient;
        _mapper = mapper;
        _logger = logger;
    }
    
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

    // GET: /Customers/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

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

    public class CorrectHeadersRequest
    {
        public List<string> OriginalHeaders { get; set; } = new();
        public List<string> CorrectHeaders { get; set; } = new();
    }
}