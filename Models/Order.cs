namespace RestaurantOrders.Models;
public class Order
{
    public int OrderId { get; set; }
    public int TableNumber { get; set; }
    public int? ServerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Server? Server { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
