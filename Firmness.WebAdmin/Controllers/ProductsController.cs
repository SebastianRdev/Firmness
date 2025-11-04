using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Products;

namespace Firmness.WebAdmin.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(
        IProductService productService,
        IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    // GET: /Products?page=1
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _productService.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(new List<ProductDto>());
        }

        ViewData["CurrentPage"] = page;
        return View(result.Data);
    }

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // GET: /Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createDto);
        }

        var result = await _productService.CreateAsync(createDto);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(createDto);
        }

        TempData["Success"] = $"Product '{result.Data.Name}' created successfully";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var updateDto = _mapper.Map<UpdateProductDto>(result.Data);

        return View(updateDto);
    }

    // POST: /Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductDto updateDto)
    {
        if (id != updateDto.Id)
        {
            TempData["Error"] = "ID does not match";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(updateDto);
        }

        var result = await _productService.UpdateAsync(updateDto);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(updateDto);
        }

        TempData["Success"] = $"Product '{{result.Data.Name}}' successfully updated";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Products/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // POST: /Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productService.DeleteAsync(id);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
        }
        else
        {
            TempData["Success"] = "Product successfully removed";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Products/Search?term=cemento&page=1
    public async Task<IActionResult> Search(string term, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _productService.SearchAsync(term);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        ViewData["SearchTerm"] = term;
        ViewData["CurrentPage"] = page;
        return View("Index", result.Data);
    }
}