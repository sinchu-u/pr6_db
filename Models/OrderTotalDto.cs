namespace RestaurantOrders.Models;
public class OrderTotalDto
{
    public int OrderId { get; set; }
    public int TableNumber { get; set; }
    public decimal Total { get; set; }
}
