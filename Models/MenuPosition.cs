namespace RestaurantOrders.Models;
public class MenuPosition
{
    public int PositionId { get; set; }
    public string PositionName { get; set; } = null!;
    public double PositionAmount { get; set; }
    public decimal PositionPrice { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
