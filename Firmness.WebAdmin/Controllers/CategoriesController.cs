namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Categories;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryApiClient _categoryApiClient;
    private readonly IMapper _mapper;

    public CategoriesController(
        ICategoryApiClient categoryApiClient,
        IMapper mapper)
    {
        _categoryApiClient = categoryApiClient;
        _mapper = mapper;
    }
    
    // GET: /Categories?page=1
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _categoryApiClient.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<CategoryViewModel>());
        }

        var viewModels = _mapper.Map<List<CategoryViewModel>>(result.Data);

        ViewData["CurrentPage"] = page;
        return View(viewModels);
    }

    // GET: /Categories/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Map ViewModel -> DTO expected by the API
        var createDto = _mapper.Map<CreateCategoryDto>(model);

        var result = await _categoryApiClient.CreateAsync(createDto);
        
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["Success"] = $"Category '{result.Data.Name}' created successfully";
        return RedirectToAction(nameof(Index));
    }
    
    // GET: /Categories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _categoryApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var viewModel = _mapper.Map<EditCategoryViewModel>(result.Data);

        return View(viewModel);
    }

    // POST: /Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
    {
        if (id != model.Id)
        {
            TempData["Error"] = "ID does not match";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var updateDto = _mapper.Map<UpdateCategoryDto>(model);

        var result = await _categoryApiClient.UpdateAsync(updateDto);
        
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            TempData["Error"] = result.ErrorMessage;

            return View(model);
        }

        TempData["Success"] = $"Category '{result.Data.Name}' successfully updated";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }

        return Json(new { success = true, category = result.Data });
    }

    // POST: /Categories/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _categoryApiClient.DeleteAsync(id);
    
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }
    
        return Json(new { success = true });
    }


    // GET: /Categories/Search?term=cemento&page=1
    public async Task<IActionResult> Search(string term, int page = 1)
    {
        var result = await _categoryApiClient.SearchAsync(term);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        ViewData["SearchTerm"] = term;
        ViewData["CurrentPage"] = page;
        return View("Index", _mapper.Map<List<CategoryViewModel>>(result.Data));
    }
}