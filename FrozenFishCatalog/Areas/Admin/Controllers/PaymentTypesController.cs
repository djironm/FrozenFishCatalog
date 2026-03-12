using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PaymentTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public PaymentTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Métodos de Pago";
        var types = await _context.PaymentTypes.OrderBy(p => p.DisplayOrder).ToListAsync();
        return View(types);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nuevo Método de Pago";
        return View(new PaymentType());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentType paymentType)
    {
        if (!ModelState.IsValid) { ViewData["Title"] = "Nuevo Método de Pago"; return View(paymentType); }
        _context.PaymentTypes.Add(paymentType);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Método de pago \"{paymentType.Name}\" creado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var type = await _context.PaymentTypes.FindAsync(id);
        if (type == null) return NotFound();
        ViewData["Title"] = "Editar Método de Pago";
        return View(type);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentType paymentType)
    {
        if (id != paymentType.Id) return BadRequest();
        if (!ModelState.IsValid) { ViewData["Title"] = "Editar Método de Pago"; return View(paymentType); }
        _context.Update(paymentType);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Método de pago actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var type = await _context.PaymentTypes.FindAsync(id);
        if (type == null) return NotFound();
        _context.PaymentTypes.Remove(type);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Método de pago eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var type = await _context.PaymentTypes.FindAsync(id);
        if (type == null) return NotFound();
        type.IsActive = !type.IsActive;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
