namespace FrozenFishCatalog.Models;

public class ProductWeight
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int WeightKg { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
