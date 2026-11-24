namespace RestaurantOrders.Models;
public class Server
{
    public int ServerId { get; set; }
    public string ServerName { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
