using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class InventoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Inventario";
        var weights = await _context.ProductWeights
            .Include(pw => pw.Product)
                .ThenInclude(p => p.Category)
            .OrderBy(pw => pw.Product.Category.Name)
            .ThenBy(pw => pw.Product.Name)
            .ThenBy(pw => pw.WeightKg)
            .ToListAsync();
        return View(weights);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(int id, int stockQuantity)
    {
        var weight = await _context.ProductWeights.FindAsync(id);
        if (weight == null) return NotFound();
        weight.StockQuantity = Math.Max(0, stockQuantity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Stock actualizado.";
        return RedirectToAction(nameof(Index));
    }
}
