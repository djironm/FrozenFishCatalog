namespace FrozenFishCatalog.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ShippingTerms { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<ProductWeight> Weights { get; set; } = new List<ProductWeight>();
}
