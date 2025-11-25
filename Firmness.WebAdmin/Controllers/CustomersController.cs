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
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _customerApiClient.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<CustomerViewModel>());
        }

        var viewModels = _mapper.Map<List<CustomerViewModel>>(result.Data);

        ViewData["CurrentPage"] = page;
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
}