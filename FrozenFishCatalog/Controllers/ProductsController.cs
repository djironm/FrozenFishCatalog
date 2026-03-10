using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Weights)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
            ViewBag.CurrentCategory = await _context.Categories.FindAsync(categoryId.Value);
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        var products = await query.ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Weights)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var viewModel = new ProductDetailsViewModel
        {
            Product = product,
            SelectedWeightId = product.Weights.FirstOrDefault()?.Id ?? 0
        };

        return View(viewModel);
    }

    public IActionResult Category(int id)
    {
        return RedirectToAction(nameof(Index), new { categoryId = id });
    }
}
