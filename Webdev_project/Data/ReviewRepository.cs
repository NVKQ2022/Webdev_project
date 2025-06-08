using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Data
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IMongoCollection<Review> reviewCollection;
        public ReviewRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            reviewCollection = database.GetCollection<Review>(settings.Value.ReviewCollectionName);
        }
        public async Task CreateReviewAsync(Review review)
        {
            try
            {
                review.CreatedTime = DateTime.UtcNow;
                await reviewCollection.InsertOneAsync(review);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB Insert Error: " + ex.Message);
                throw;
            }
        }

        public async Task<List<Review>> GetReviewsByProductIdAsync(string productId)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);
            return await reviewCollection.Find(filter).SortByDescending(r => r.CreatedTime).ToListAsync();
        }

        // READ reviews by user
        public async Task<List<Review>> GetReviewsByUserIdAsync(int userId)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.UserID, userId);
            return await reviewCollection.Find(filter).ToListAsync();
        }
    }
}
