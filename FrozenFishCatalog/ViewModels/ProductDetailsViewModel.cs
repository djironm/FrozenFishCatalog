using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.ViewModels;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    public int SelectedWeightId { get; set; }
    public int Quantity { get; set; } = 1;
}
