using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.ViewModels;

public class HomeViewModel
{
    public List<Category> Categories { get; set; } = new();
    public List<Product> FeaturedProducts { get; set; } = new();
}
