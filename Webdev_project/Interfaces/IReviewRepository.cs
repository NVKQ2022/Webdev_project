using MongoDB.Driver;
using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface IReviewRepository
    {
        Task CreateReviewAsync(Review review);


        Task<List<Review>> GetReviewsByProductIdAsync(string productId);



        Task<List<Review>> GetReviewsByUserIdAsync(int userId);
        
    }
}
