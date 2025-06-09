using Webdev_project.Models;
namespace Webdev_project.Interfaces

{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string id);
        Task<List<Order>> GetOrdersByUserAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(string id, string newStatus);

        Task DeleteOrderAsync(string id);

        Task<bool> CancelOrder(string orderId);
        Task<Order> ChooseProductToReview(string orderId);
    }
}
