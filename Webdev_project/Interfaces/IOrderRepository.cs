namespace Webdev_project.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string id);
        Task<List<Order>> GetOrdersByUserAsync(string userId);
        Task<bool> UpdateOrderStatusAsync(string id, string newStatus);

        Task DeleteOrderAsync(string id);


    }
}
