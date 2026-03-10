using Microsoft.AspNetCore.Identity;

namespace FrozenFishCatalog.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
