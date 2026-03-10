using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Quantity);
}

public class CartItemViewModel
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int WeightKg { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}
