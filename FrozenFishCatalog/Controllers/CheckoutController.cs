using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrozenFishCatalog.Data;
using FrozenFishCatalog.Models;
using FrozenFishCatalog.ViewModels;

namespace FrozenFishCatalog.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var viewModel = new CheckoutViewModel
        {
            Cart = new CartViewModel
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
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(CheckoutViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.ProductWeight)
                .ThenInclude(pw => pw.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        // Repopulate cart for validation errors
        model.Cart = new CartViewModel
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

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        // Mock payment processing - always succeeds
        var transactionId = $"MOCK-{Guid.NewGuid():N}".Substring(0, 20).ToUpper();

        // Create order
        var order = new Order
        {
            UserId = userId!,
            TotalAmount = model.Cart.TotalPrice,
            ShippingAddress = model.ShippingAddress,
            City = model.City,
            PostalCode = model.PostalCode,
            Country = model.Country,
            Status = OrderStatus.PaymentReceived,
            PaymentTransactionId = transactionId,
            Items = cartItems.Select(c => new OrderItem
            {
                ProductWeightId = c.ProductWeightId,
                Quantity = c.Quantity,
                UnitPrice = c.ProductWeight.Price
            }).ToList()
        };

        _context.Orders.Add(order);

        // Clear cart
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Confirmation), new { orderId = order.Id });
    }

    public async Task<IActionResult> Confirmation(int orderId)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductWeight)
                    .ThenInclude(pw => pw.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
