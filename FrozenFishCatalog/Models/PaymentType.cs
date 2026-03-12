namespace FrozenFishCatalog.Models;

public class PaymentType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-credit-card";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
