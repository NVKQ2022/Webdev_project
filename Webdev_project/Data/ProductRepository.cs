using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using MongoDB.Bson;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static SearchController;
using System.Text.RegularExpressions;
public class ProductRepository:IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _products = database.GetCollection<Product>(settings.Value.ProductCollectionName);
    }
    public async Task<List<Product>> GetSuggestions( string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            //return Ok(new List<ProductSuggestionDto>());
            return null;
        }
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase | RegexOptions.Multiline)));

        //Lấy tối đa 7 kết quả gợi ý
        var productsFromDb = await _products.Find(filter)
                                                     .Limit(7)
                                                     .Project(p => new Product
                                                     {
                                                         ProductId = p.ProductId,
                                                         Name = p.Name,
                                                         Price = p.Price,
                                                         ImageURL = p.ImageURL
                                                     })
                                                     .ToListAsync();
        return productsFromDb;
    }
    public async Task<List<Product>> GetAllAsync()
    {
        return await _products.Find(p => true).ToListAsync();
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.ProductId == id).FirstOrDefaultAsync();
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
        await _products.ReplaceOneAsync(p => p.ProductId == id, updatedProduct);
    }

    public async Task DeleteAsync(string id)
    {
        await _products.DeleteOneAsync(p => p.ProductId == id);
    }



    public async Task ConvertKhoToStringAsync()
    {
        var collection = _products.Database.GetCollection<BsonDocument>(_products.CollectionNamespace.CollectionName);

        var pipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(
                new BsonDocument("$set", new BsonDocument("Detail.Kho", new BsonDocument("$toString", "$Detail.Kho")))
            );

        var filter = Builders<BsonDocument>.Filter.Exists("Detail.Kho");

        await collection.UpdateManyAsync(filter, pipeline);
    }
    public async Task EnsureColorIsArrayAsync()
    {
        var collection = _products.Database.GetCollection<BsonDocument>(_products.CollectionNamespace.CollectionName);

        var filter = Builders<BsonDocument>.Filter.Or(
            Builders<BsonDocument>.Filter.Exists("Color", false), // Color does not exist
            Builders<BsonDocument>.Filter.Not(
                Builders<BsonDocument>.Filter.Type("Color", BsonType.Array) // Color is not an array
            )
        );

        var update = Builders<BsonDocument>.Update.Set("Color", new BsonArray());

        await collection.UpdateManyAsync(filter, update);
    }

    public async Task AddSoldFieldWithRandomValueAsync()
    {
        var collection = _products.Database.GetCollection<BsonDocument>(_products.CollectionNamespace.CollectionName);

        var allDocuments = await collection.Find(new BsonDocument()).ToListAsync();

        var random = new Random();

        var tasks = new List<Task>();

        foreach (var doc in allDocuments)
        {
            var id = doc["_id"];
            int soldValue = random.Next(0, 101); // 0 to 100 inclusive

            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var update = Builders<BsonDocument>.Update.Set("Sold", soldValue);

            tasks.Add(collection.UpdateOneAsync(filter, update));
        }

        await Task.WhenAll(tasks);
    }
    public async Task changeProductId()
    {
        var filter = Builders<Product>.Filter.Exists("ProductId");
        var update = Builders<Product>.Update.Rename("ProductId", "ProductId");
        await _products.UpdateManyAsync(filter, update);

        await _products.UpdateManyAsync(filter, update);
    }
// Search sản phẩm - Phong
    public async Task<List<Product>> SearchByNameAsync(string query)
    {
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(query, "i"));
        return await _products.Find(filter).ToListAsync();
    }
}
