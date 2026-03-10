using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.ProductWeight)
                .ThenInclude(pw => pw.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var viewModel = new CartViewModel
        {
            Items = cartItems.Select(c => new CartItemViewModel
            {
                CartItemId = c.Id,
                ProductId = c.ProductWeight.Product.Id,
                ProductName = c.ProductWeight.Product.Name,
                ProductImage = c.ProductWeight.Product.ImageUrl,
                WeightKg = c.ProductWeight.WeightKg,
                UnitPrice = c.ProductWeight.Price,
                Quantity = c.Quantity
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productWeightId, int quantity = 1)
    {
        var userId = _userManager.GetUserId(User);

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductWeightId == productWeightId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId!,
                ProductWeightId = productWeightId,
                Quantity = quantity
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int cartItemId, int quantity)
    {
        var userId = _userManager.GetUserId(User);
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (cartItem != null)
        {
            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = _userManager.GetUserId(User);
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<int> GetCartCount()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return 0;

        return await _context.CartItems
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);
    }
}
