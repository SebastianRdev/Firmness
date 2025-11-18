namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Categories;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models;
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
        var dto = new CreateCategoryDto
        {
            Name = model.Name
        };

        var result = await _categoryApiClient.CreateAsync(dto);
        
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["Success"] = $"Category '{result.Data.Name}' created successfully";
        return RedirectToAction(nameof(Index));
    }
}