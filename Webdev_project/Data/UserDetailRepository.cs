using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Webdev_project.Interfaces;
using Webdev_project.Models;
namespace Webdev_project.Data
{

    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly IMongoCollection<UserDetail> _userDetail;

        public UserDetailRepository(IOptions<MongoDbSettings> settings)
        {
            
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _userDetail = database.GetCollection<UserDetail>(settings.Value.UserDetailCollectionName);
        }

        public async Task AddUserDetailAsync(UserDetail user)
        {
            await _userDetail.InsertOneAsync(user);
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);

            var projection = Builders<UserDetail>.Projection.Include(u => u.Cart);

            var result = await _userDetail.Find(filter).Project<UserDetail>(projection).FirstOrDefaultAsync();

            return result?.Cart ?? new List<CartItem>();
        }


        public async Task AddCartItemAsync(int userId, CartItem item)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Push(u => u.Cart, item);
            await _userDetail.UpdateOneAsync(filter, update);
        }

        //public async Task<int?> UpdateCartItemQuantityAsync(int userId, string productId, int changeAmount)
        //{
        //    var filter = Builders<UserDetail>.Filter.And(
        //        Builders<UserDetail>.Filter.Eq(u => u.UserId, userId),
        //        Builders<UserDetail>.Filter.ElemMatch(u => u.Cart, c => c.ProductId == productId)
        //    );

        //    var update = Builders<UserDetail>.Update.Inc("Cart.$.Quantity", changeAmount);

        //    var result = await _userDetail.UpdateOneAsync(filter, update);

        //    if (result.ModifiedCount > 0)
        //    {
        //        // Fetch the updated quantity
        //        var updatedUser = await _userDetail.Find(
        //            Builders<UserDetail>.Filter.Eq(u => u.UserId, userId)
        //        ).FirstOrDefaultAsync();

        //        var item = updatedUser?.Cart?.FirstOrDefault(c => c.ProductId == productId);
        //        return item?.Quantity;
        //    }

        //    return null; // not updated
        //}
        public async Task<int?> UpdateCartItemQuantityAsync(int userId, string productId, int changeAmount)
        {
            // Step 1: Get current quantity
            var user = await _userDetail.Find(
                Builders<UserDetail>.Filter.Eq(u => u.UserId, userId)
            ).FirstOrDefaultAsync();

            var item = user?.Cart?.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return null;

            int newQuantity = item.Quantity + changeAmount;

            // Step 2: Check if the new quantity would be <= 1
            if (newQuantity <= 0)
            {
                return item.Quantity; // Don't allow reducing to 1 or below
            }

            // Step 3: Proceed with update
            var filter = Builders<UserDetail>.Filter.And(
                Builders<UserDetail>.Filter.Eq(u => u.UserId, userId),
                Builders<UserDetail>.Filter.ElemMatch(u => u.Cart, c => c.ProductId == productId)
            );

            var update = Builders<UserDetail>.Update.Inc("Cart.$.Quantity", changeAmount);

            var result = await _userDetail.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return newQuantity;
            }

            return null;
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



        //public async Task AddReceiveInfoAsync(int userId, ReceiveInfo newInfo)
        //{
        //    var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
        //    var update = Builders<UserDetail>.Update.Push(u => u.ReceiveInfo, newInfo);

        //    await _userDetail.UpdateOneAsync(filter, update);
        //}

        //public async Task<bool> DeleteReceiveInfoAsync(int userId, ReceiveInfo targetInfo)
        //{
        //    var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);


        //    var update = Builders<UserDetail>.Update.PullFilter(u => u.ReceiveInfo,
        //        Builders<ReceiveInfo>.Filter.And(
        //            Builders<ReceiveInfo>.Filter.Eq(r => r.Name, targetInfo.Name),
        //            Builders<ReceiveInfo>.Filter.Eq(r => r.Phone, targetInfo.Phone),
        //            Builders<ReceiveInfo>.Filter.Eq(r => r.Address, targetInfo.Address)
        //        )
        //    );

        //    var result = await _userDetail.UpdateOneAsync(filter, update);
        //    return result.ModifiedCount > 0;
        //}

        public async Task AddReceiveInfoAsync(int userId, ReceiveInfo newInfo)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Push("ReceiveInfo", newInfo); // string path works too

            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task<bool> DeleteReceiveInfoAsync(int userId, ReceiveInfo targetInfo)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);

            var nestedFilter = Builders<ReceiveInfo>.Filter.And(
                Builders<ReceiveInfo>.Filter.Eq(r => r.Name, targetInfo.Name),
                Builders<ReceiveInfo>.Filter.Eq(r => r.Phone, targetInfo.Phone),
                Builders<ReceiveInfo>.Filter.Eq(r => r.Address, targetInfo.Address)
            );

            // Use "ReceiveInfo" as a string field path
            var update = Builders<UserDetail>.Update.PullFilter("ReceiveInfo", nestedFilter);

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
