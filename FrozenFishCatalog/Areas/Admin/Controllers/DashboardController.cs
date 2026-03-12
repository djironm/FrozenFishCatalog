using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new AdminDashboardViewModel
        {
            TotalOrders = await _context.Orders.CountAsync(),
            PendingOrders = await _context.Orders.CountAsync(o =>
                o.Status == OrderStatus.Pending || o.Status == OrderStatus.PaymentReceived),
            TotalRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            TotalProducts = await _context.Products.CountAsync(),
            LowStockCount = await _context.ProductWeights.CountAsync(pw => pw.StockQuantity < 10),
            RecentOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .Take(7)
                .ToListAsync()
        };

        ViewData["Title"] = "Dashboard";
        return View(vm);
    }
}
