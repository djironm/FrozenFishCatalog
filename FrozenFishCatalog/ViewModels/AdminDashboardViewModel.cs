using FrozenFishCatalog.Models;

namespace FrozenFishCatalog.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockCount { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
}
