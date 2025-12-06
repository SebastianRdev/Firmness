using Firmness.Application.DTOs.Categories;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Categories;

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

/// <summary>
/// Controller for managing categories in the Web Admin interface.
/// </summary>
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryApiClient _categoryApiClient;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesController"/> class.
    /// </summary>
    /// <param name="categoryApiClient">The category API client.</param>
    /// <param name="mapper">The object mapper.</param>
    public CategoriesController(
        ICategoryApiClient categoryApiClient,
        IMapper mapper)
    {
        _categoryApiClient = categoryApiClient;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Displays a list of categories.
    /// </summary>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>The index view with a list of categories.</returns>
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

    /// <summary>
    /// Displays the view for creating a new category.
    /// </summary>
    /// <returns>The create view.</returns>
    // GET: /Categories/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles the creation of a new category.
    /// </summary>
    /// <param name="model">The create category view model.</param>
    /// <returns>Redirects to the index view on success, or returns the create view with errors.</returns>
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
    
    /// <summary>
    /// Displays the view for editing an existing category.
    /// </summary>
    /// <param name="id">The ID of the category to edit.</param>
    /// <returns>The edit view with the category data.</returns>
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

    /// <summary>
    /// Handles the update of an existing category.
    /// </summary>
    /// <param name="id">The ID of the category to update.</param>
    /// <param name="model">The edit category view model.</param>
    /// <returns>Redirects to the index view on success, or returns the edit view with errors.</returns>
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

    /// <summary>
    /// Retrieves category details for deletion confirmation via AJAX.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns>A JSON result containing the category data or an error message.</returns>
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

    /// <summary>
    /// Confirms the deletion of a category.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
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


    /// <summary>
    /// Searches for categories based on a search term.
    /// </summary>
    /// <param name="term">The search term.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>The index view with the search results.</returns>
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