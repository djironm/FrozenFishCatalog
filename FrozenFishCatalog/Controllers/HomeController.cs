using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            Categories = await _context.Categories.ToListAsync(),
            FeaturedProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Weights)
                .Take(10)
                .ToListAsync()
        };
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
