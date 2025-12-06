using Firmness.Application.DTOs.Products;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Products;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
namespace Firmness.WebAdmin.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Products;
using Firmness.WebAdmin.ApiClients;
using Firmness.WebAdmin.Models.Products;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

/// <summary>
/// Controller for managing products in the Web Admin interface.
/// </summary>
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductApiClient _productApiClient;
    private readonly ICategoryApiClient _categoryApiClient;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="productApiClient">The product API client.</param>
    /// <param name="categoryApiClient">The category API client.</param>
    /// <param name="mapper">The object mapper.</param>
    public ProductsController(
        IProductApiClient productApiClient,
        ICategoryApiClient categoryApiClient,
        IMapper mapper)
    {
        _productApiClient = productApiClient;
        _categoryApiClient = categoryApiClient;
        _mapper = mapper;
    }

    /// <summary>
    /// Displays a list of products.
    /// </summary>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>The index view with the list of products.</returns>
    // GET: /Products?page=1
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _productApiClient.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<ProductViewModel>());
        }

        var viewModels = _mapper.Map<List<ProductViewModel>>(result.Data);

        ViewData["CurrentPage"] = page;
        return View(viewModels);
    }

    /// <summary>
    /// Displays the details of a specific product.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <returns>The details view with the product data.</returns>
    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var result = await _productApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    /// <summary>
    /// Displays the view for creating a new product.
    /// </summary>
    /// <returns>The create view with category options.</returns>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var result = await _categoryApiClient.GetAllAsync();
        
        if (!result.IsSuccess || result.Data == null)
        {
            TempData["Error"] = "Could not load categories.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new CreateProductViewModel
        {
            Categories = result.Data.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
        };

        return View(viewModel);
    }


    /// <summary>
    /// Handles the creation of a new product.
    /// </summary>
    /// <param name="model">The create product view model.</param>
    /// <returns>Redirects to the index view on success, or returns the create view with errors.</returns>
    // POST: /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // If validation fails, reload the categories
            var categories = await _categoryApiClient.GetAllAsync();
            
            model.Categories = categories.Data.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            return View(model);
        }
        
        // Map ViewModel -> DTO expected by the API
        var createDto = _mapper.Map<CreateProductDto>(model);

        var result = await _productApiClient.CreateAsync(createDto);
        
        if (!result.IsSuccess)
        {
            // If there is an error, we reload the categories so that the dropdown is not empty.
            var categoryResult = await _categoryApiClient.GetAllAsync();
            model.Categories = categoryResult.IsSuccess && categoryResult.Data != null
                ? categoryResult.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                : Enumerable.Empty<SelectListItem>();

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["Success"] = $"Product '{result.Data.Name}' created successfully";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the view for editing an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to edit.</param>
    /// <returns>The edit view with the product data and category options.</returns>
    // GET: /Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _productApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var viewModel = _mapper.Map<EditProductViewModel>(result.Data);

        var categoriesResult = await _categoryApiClient.GetAllAsync();

        if (categoriesResult.IsSuccess && categoriesResult.Data != null)
        {
            viewModel.Categories = categoriesResult.Data.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == viewModel.CategoryId
            });
        }
        else
        {
            viewModel.Categories = Enumerable.Empty<SelectListItem>();
        }

        return View(viewModel);
    }

    /// <summary>
    /// Handles the update of an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="model">The edit product view model.</param>
    /// <returns>Redirects to the index view on success, or returns the edit view with errors.</returns>
    // POST: /Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditProductViewModel model)
    {
        if (id != model.Id)
        {
            TempData["Error"] = "ID does not match";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            var categories = await _categoryApiClient.GetAllAsync();

            model.Categories = categories.IsSuccess && categories.Data != null
                ? categories.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CategoryId
                })
                : Enumerable.Empty<SelectListItem>();

            return View(model);
        }
        
        var updateDto = _mapper.Map<UpdateProductDto>(model);

        var result = await _productApiClient.UpdateAsync(updateDto);
        
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            TempData["Error"] = result.ErrorMessage;

            // Reload categories (same as above)
            var categories = await _categoryApiClient.GetAllAsync();

            model.Categories = categories.IsSuccess && categories.Data != null
                ? categories.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CategoryId
                })
                : Enumerable.Empty<SelectListItem>();

            return View(model); //
        }

        TempData["Success"] = $"Product '{result.Data.Name}' successfully updated";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Retrieves product details for deletion confirmation via AJAX.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>A JSON result containing the product data or an error message.</returns>
    // GET: /Products/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productApiClient.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }

        return Json(new { success = true, product = result.Data });
    }

    /// <summary>
    /// Confirms the deletion of a product.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    // POST: /Products/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productApiClient.DeleteAsync(id);
    
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.ErrorMessage });
        }
    
        return Json(new { success = true });
    }


    /// <summary>
    /// Searches for products based on a search term.
    /// </summary>
    /// <param name="term">The search term.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>The index view with the search results.</returns>
    // GET: /Products/Search?term=cemento&page=1
    public async Task<IActionResult> Search(string term, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _productApiClient.SearchAsync(term);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        ViewData["SearchTerm"] = term;
        ViewData["CurrentPage"] = page;
        return View("Index", _mapper.Map<List<ProductViewModel>>(result.Data));
    }
}