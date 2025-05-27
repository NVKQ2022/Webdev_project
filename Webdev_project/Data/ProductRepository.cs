using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Webdev_project.Models;
using Webdev_project.Interfaces;

public class ProductRepository:IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _products = database.GetCollection<Product>(settings.Value.ProductCollectionName);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _products.Find(p => true).ToListAsync();
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.ProductID == id).FirstOrDefaultAsync();
    }

    public async Task<Product> GetByNameAsync(string name)
    {
        return await _products.Find(p => p.Name ==name).FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetByCategoryAsync(string category)
    {
        return await _products.Find(p => p.Category == category).ToListAsync();
    }

    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _products.Distinct<string>("Category", FilterDefinition<Product>.Empty).ToListAsync();
    }


    public async Task AddAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateAsync(string id, Product updatedProduct)
    {
        await _products.ReplaceOneAsync(p => p.ProductID == id, updatedProduct);
    }

    public async Task DeleteAsync(string id)
    {
        await _products.DeleteOneAsync(p => p.ProductID == id);
    }
}
