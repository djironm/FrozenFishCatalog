namespace FrozenFishCatalog.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductWeightId { get; set; }
    public ProductWeight ProductWeight { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
