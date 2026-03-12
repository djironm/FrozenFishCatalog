using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Productos";
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Weights)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Nuevo Producto";
        var vm = new AdminProductFormViewModel
        {
            Categories = await GetCategoriesSelectList(),
            Stock1kg = 50, Stock2kg = 30, Stock3kg = 20
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategoriesSelectList();
            ViewData["Title"] = "Nuevo Producto";
            return View(vm);
        }

        var product = new Product
        {
            Name = vm.Name,
            Description = vm.Description,
            ImageUrl = vm.ImageUrl,
            ShippingTerms = vm.ShippingTerms,
            CategoryId = vm.CategoryId
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _context.ProductWeights.AddRange(
            new ProductWeight { ProductId = product.Id, WeightKg = 1, Price = vm.Price1kg, StockQuantity = vm.Stock1kg },
            new ProductWeight { ProductId = product.Id, WeightKg = 2, Price = vm.Price2kg, StockQuantity = vm.Stock2kg },
            new ProductWeight { ProductId = product.Id, WeightKg = 3, Price = vm.Price3kg, StockQuantity = vm.Stock3kg }
        );
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Producto \"{product.Name}\" creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.Include(p => p.Weights).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        var w1 = product.Weights.FirstOrDefault(w => w.WeightKg == 1);
        var w2 = product.Weights.FirstOrDefault(w => w.WeightKg == 2);
        var w3 = product.Weights.FirstOrDefault(w => w.WeightKg == 3);

        var vm = new AdminProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ShippingTerms = product.ShippingTerms,
            CategoryId = product.CategoryId,
            Categories = await GetCategoriesSelectList(),
            Weight1kgId = w1?.Id ?? 0, Price1kg = w1?.Price ?? 0, Stock1kg = w1?.StockQuantity ?? 0,
            Weight2kgId = w2?.Id ?? 0, Price2kg = w2?.Price ?? 0, Stock2kg = w2?.StockQuantity ?? 0,
            Weight3kgId = w3?.Id ?? 0, Price3kg = w3?.Price ?? 0, Stock3kg = w3?.StockQuantity ?? 0,
        };

        ViewData["Title"] = "Editar Producto";
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategoriesSelectList();
            ViewData["Title"] = "Editar Producto";
            return View(vm);
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = vm.Name;
        product.Description = vm.Description;
        product.ImageUrl = vm.ImageUrl;
        product.ShippingTerms = vm.ShippingTerms;
        product.CategoryId = vm.CategoryId;

        await UpsertWeight(vm.Weight1kgId, id, 1, vm.Price1kg, vm.Stock1kg);
        await UpsertWeight(vm.Weight2kgId, id, 2, vm.Price2kg, vm.Stock2kg);
        await UpsertWeight(vm.Weight3kgId, id, 3, vm.Price3kg, vm.Stock3kg);

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Producto \"{product.Name}\" actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Producto eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task UpsertWeight(int weightId, int productId, int kg, decimal price, int stock)
    {
        if (weightId > 0)
        {
            var w = await _context.ProductWeights.FindAsync(weightId);
            if (w != null) { w.Price = price; w.StockQuantity = stock; }
        }
        else
        {
            _context.ProductWeights.Add(new ProductWeight
            {
                ProductId = productId, WeightKg = kg, Price = price, StockQuantity = stock
            });
        }
    }

    private async Task<List<SelectListItem>> GetCategoriesSelectList() =>
        await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
}
