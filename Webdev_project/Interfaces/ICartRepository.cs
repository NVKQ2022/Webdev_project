// Interfaces/ICartRepository.cs
using System.Threading.Tasks;

public interface ICartRepository
{
    Task<Cart> GetCartByUserIdAsync(int userId);
    Task AddOrUpdateCartAsync(Cart cart);
    Task ClearCartAsync(int userId);

}
