namespace RestaurantOrders.Models;
public class OrderItem
{
    public int ItemId { get; set; }
    public int OrderId { get; set; }
    public int PositionId { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; } = null!;
    public MenuPosition Position { get; set; } = null!;
}
