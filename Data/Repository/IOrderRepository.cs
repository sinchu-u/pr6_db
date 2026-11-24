using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(int tableNumber, int serverId);
    Task<Order?> UpdateAsync(int id, int tableNumber, int serverId);
    Task<bool> DeleteAsync(int id);

    Task<OrderItem> AddItemAsync(int orderId, int positionId, int quantity);
    Task<bool> UpdateItemQuantityAsync(int itemId, int quantity);
    Task<bool> RemoveItemAsync(int itemId);
}
