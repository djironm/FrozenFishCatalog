namespace FrozenFishCatalog.Models;

public class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int ProductWeightId { get; set; }
    public ProductWeight ProductWeight { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
