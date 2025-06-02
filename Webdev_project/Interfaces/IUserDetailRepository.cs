using MongoDB.Driver;
using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface IUserDetailRepository
    {
        Task AddUserDetailAsync(UserDetail user);
        Task UpdatePhoneNumberAsync(string userId, string newPhoneNumber);

        Task AddCartItemAsync(int userId, CartItem item);

        Task RemoveCartItemAsync(int userId, string productId);

       
        Task AddReceiveInfoAsync(int userId, ReceiveInfo newInfo);
        Task<bool> DeleteReceiveInfoAsync(int userId, ReceiveInfo targetInfo);
        

        Task UpdateBankingInfoAsync(int userId, string newAccount, string newCard);
    }
}
