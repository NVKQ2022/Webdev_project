using MongoDB.Driver;
using Webdev_project.Interfaces;
using Webdev_project.Models;
namespace Webdev_project.Data
{

    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly IMongoCollection<UserDetail> _userDetail;

        public async Task AddUserDetailAsync(UserDetail user)
        {
            await _userDetail.InsertOneAsync(user);
        }

        public async Task AddCartItemAsync(int userId, CartItem item)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Push(u => u.Cart, item);
            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task RemoveCartItemAsync(int userId, string productId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.PullFilter(
                u => u.Cart,
                c => c.ProductId == productId
            );
            await _userDetail.UpdateOneAsync(filter, update);
        }



        public async Task AddReceiveInfoAsync(int userId, ReceiveInfo newInfo)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Push(u => u.ReceiveInfo, newInfo);

            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task<bool> DeleteReceiveInfoAsync(int userId, ReceiveInfo targetInfo)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);

            var update = Builders<UserDetail>.Update.PullFilter(u => u.ReceiveInfo,
                Builders<ReceiveInfo>.Filter.And(
                    Builders<ReceiveInfo>.Filter.Eq(r => r.Name, targetInfo.Name),
                    Builders<ReceiveInfo>.Filter.Eq(r => r.Phone, targetInfo.Phone),
                    Builders<ReceiveInfo>.Filter.Eq(r => r.Address, targetInfo.Address)
                )
            );

            var result = await _userDetail.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task UpdateBankingInfoAsync(int userId, string newAccount, string newCard)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);

            var update = Builders<UserDetail>.Update
                .Set(u => u.Banking.BankAccount, newAccount)
                .Set(u => u.Banking.CreditCard, newCard);

            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task UpdatePhoneNumberAsync(string userId, string newPhoneNumber)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, int.Parse(userId));
            var update = Builders<UserDetail>.Update.Set(u => u.PhoneNumber, newPhoneNumber);
            await _userDetail.UpdateOneAsync(filter, update);
        }
    }
}
