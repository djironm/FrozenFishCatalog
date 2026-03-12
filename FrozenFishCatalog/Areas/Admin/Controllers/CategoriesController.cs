using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Categorías";
        var categories = await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nueva Categoría";
        return View(new Category());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) { ViewData["Title"] = "Nueva Categoría"; return View(category); }
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Categoría \"{category.Name}\" creada.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        ViewData["Title"] = "Editar Categoría";
        return View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (!ModelState.IsValid) { ViewData["Title"] = "Editar Categoría"; return View(category); }
        _context.Update(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Categoría \"{category.Name}\" actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        if (category.Products.Any())
        {
            TempData["Error"] = "No se puede eliminar una categoría que tiene productos asignados.";
            return RedirectToAction(nameof(Index));
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoría eliminada.";
        return RedirectToAction(nameof(Index));
    }
}
