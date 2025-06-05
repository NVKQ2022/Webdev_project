using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Data
{
    public class OrderRepository:IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _orders = database.GetCollection<Order>(settings.Value.OrderCollectionName);
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(o => o.OrderID == id).FirstOrDefaultAsync();
         
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _orders.Find(o => o.UserID == userId).ToListAsync();

        }

        public async Task AddOrderAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }
        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderID, orderId);
            var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
            var result = await _orders.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _orders.DeleteOneAsync(o => o.OrderID == id);
        }
    }
}
