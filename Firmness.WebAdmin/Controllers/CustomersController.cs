namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Customers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IMapper _mapper;

    public CustomersController(
        ICustomerApiClient customerApiClient,
        IMapper mapper)
    {
        _customerApiClient = customerApiClient;
        _mapper = mapper;
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

        // Map ViewModel → DTO
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
    /*
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file selected." });

        var result = await _customerApiClient.ExtractHeadersFromExcelAsync(file);

        if (!result.IsSuccess)
            return Json(new { success = false, message = result.ErrorMessage });

        return Json(new { success = true, headers = result.Data.OriginalHeaders });
    }
    */
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file selected." });

        //_logger.LogInformation("📁 File received: {FileName}, Size: {Size} bytes", file.FileName, file.Length);

        var result = await _customerApiClient.ExtractHeadersFromExcelAsync(file);

        //_logger.LogInformation("📊 API Result - IsSuccess: {IsSuccess}", result.IsSuccess);

        if (!result.IsSuccess)
        {
            //_logger.LogError("❌ Error from API: {ErrorMessage}", result.ErrorMessage);
            return Json(new { success = false, message = result.ErrorMessage });
        }

        if (result.Data == null)
        {
            //_logger.LogWarning("⚠️ result.Data is NULL");
            return Json(new { success = false, message = "No data returned from API" });
        }

        if (result.Data.OriginalHeaders == null)
        {
            //_logger.LogWarning("⚠️ result.Data.OriginalHeaders is NULL");
            return Json(new { success = false, message = "Headers property is null" });
        }

        //_logger.LogInformation("✅ Headers count: {Count}", result.Data.OriginalHeaders.Count);
        //_logger.LogInformation("📋 Headers: {Headers}", string.Join(", ", result.Data.OriginalHeaders));

        return Json(new { 
            success = true, 
            headers = result.Data.OriginalHeaders 
        });
    }

    /*
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "No file selected.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _customerApiClient.ExtractHeadersFromExcelAsync(file);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        // Guardar en TempData o en un lugar temporal (ej: cache) para usar luego
        TempData["ExcelHeaders"] = System.Text.Json.JsonSerializer.Serialize(result.Data.OriginalHeaders);

        // Aquí deberías devolver la vista Index (o partial) que abre un modal y muestra los headers.
        // Para no cambiar mucho, redirigimos a Index y la vista leerá TempData["ExcelHeaders"] para abrir modal.
        TempData["Success"] = "Headers extracted. Please confirm mapping.";
        return RedirectToAction(nameof(Index));
    }*/

}