using MongoDB.Driver;
using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(string id);
        Task<Product> GetByNameAsync(string name);
        Task<List<Product>> GetByCategoryAsync(string category);



        Task<List<string>> GetAllCategoriesAsync();

        Task AddAsync(Product product);

        Task UpdateAsync(string id,Product product);

        Task DeleteAsync(string id);

        Task ConvertKhoToStringAsync();

        Task EnsureColorIsArrayAsync();
        Task changeProductId();
        Task AddSoldFieldWithRandomValueAsync();
    }
}
