using MongoDB.Driver;
using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public enum UserAction
    {
        Click,
        AddToCart,
        Purchase,
        ClickCategory
    }
    public interface IUserDetailRepository
    {
        Task AddUserDetailAsync(UserDetail user);

        Task<bool> UpdateUserInfo(int userId,string name, string phoneNumber, string gender, DateTime birthDay);

        Task<UserDetail> GetUserDetailAsync(int userId);
        Task UpdatePhoneNumberAsync(int userId, string newPhoneNumber);
        Task<UserDetail> GetUserByUserId(int userId);
        Task<List<string>> GetCategoriesByPointDescending(int userId);
        Task<int> CountCartItems(int userId);
        Task<List<CartItem>> GetCartItemsAsync(int userId);

        Task<int?> UpdateCartItemQuantityAsync(int userId, string productId, int changeAmount);

        Task<int?> InsertCartItemQuantityAsync(int userId, string productId, int quantity);


        Task AddCartItemAsync(int userId, CartItem item);

        Task RemoveCartItemAsync(int userId, string productId);

        Task<List<ReceiveInfo>> GetReceiveInfoAsync(int userId);
        Task AddReceiveInfoAsync(int userId, ReceiveInfo newInfo);
        Task<bool> DeleteReceiveInfoAsync(int userId, ReceiveInfo targetInfo);
        Task InsertUserCategoriesAsync(int userId, List<string> categoryNames);
        Task UpdateBankingInfoAsync(int userId, string newAccount, string newCard);
        Task UpdateCategoryScoreAsync(int userId, string categoryName, UserAction action);
        
    }
}
