using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? status)
    {
        ViewData["Title"] = "Pedidos";
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var statusEnum))
            query = query.Where(o => o.Status == statusEnum);

        var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        ViewBag.StatusFilter = status;
        ViewBag.StatusValues = Enum.GetValues<OrderStatus>();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = $"Pedido #{id}";
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductWeight)
                    .ThenInclude(pw => pw.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        ViewBag.StatusValues = Enum.GetValues<OrderStatus>();
        return View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = status;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Estado del pedido #{id} actualizado.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
