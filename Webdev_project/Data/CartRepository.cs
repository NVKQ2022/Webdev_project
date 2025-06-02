// Data/CartRepository.cs
using MongoDB.Driver;
using System.Threading.Tasks;

public class CartRepository : ICartRepository
{
    private readonly IMongoCollection<Cart> _carts;

    public CartRepository(IMongoDatabase database)
    {
        _carts = database.GetCollection<Cart>("Carts");
    }

    public async Task<Cart> GetCartByUserIdAsync(int userId)
    {
        return await _carts.Find(c => c.UserId == userId).FirstOrDefaultAsync() ?? new Cart { UserId = userId };
    }

    public async Task AddOrUpdateCartAsync(Cart cart)
    {
        if (string.IsNullOrEmpty(cart.Id))
        {
            // Cart mới, dùng InsertOneAsync để MongoDB tự sinh _id
            await _carts.InsertOneAsync(cart);
        }
        else
        {
            // Cart đã có _id, dùng ReplaceOneAsync để update
            await _carts.ReplaceOneAsync(c => c.Id == cart.Id, cart, new ReplaceOptions { IsUpsert = true });
        }
    }


    public async Task ClearCartAsync(int userId)
    {
        await _carts.DeleteOneAsync(c => c.UserId == userId);
    }
}
