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
        public async Task<bool> UpdateUserInfo(int userId,string name, string phoneNumber, string gender, DateTime birthDay)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update
                .Set(u => u.Name, name)
                .Set(u => u.PhoneNumber, phoneNumber)
                .Set(u => u.Gender, gender)
                .Set(u => u.Birthday, birthDay);

            var result= await _userDetail.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0 && result.ModifiedCount > 0;
        }
        public async Task<UserDetail> GetUserDetailAsync(int userId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            return await _userDetail.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<UserDetail> GetUserByUserId(int userId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            return await _userDetail.Find(filter).FirstOrDefaultAsync();
        }
        public async Task AddUserDetailAsync(UserDetail user)
        {

            await _userDetail.InsertOneAsync(user);
        }

        public async Task<int>CountCartItems(int userId)
        {

            var user = await GetUserByUserId(userId); // Assuming synchronous call

            if (user == null || user.Cart == null)
                return 0;

            return user.Cart.Count; // Count of different CartItem entries
        }
        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);

            var projection = Builders<UserDetail>.Projection.Include(u => u.Cart);

            var result = await _userDetail.Find(filter).Project<UserDetail>(projection).FirstOrDefaultAsync();

            return result?.Cart ?? new List<CartItem>();
        }


        public async Task<List<string>> GetCategoriesByPointDescending(int userId)
        {
            var user = await GetUserByUserId(userId);
            if (user?.Category == null)
                return new List<string>();

            return user.Category
                       .OrderByDescending(pair => pair.Value)
                       .Select(pair => pair.Key)
                       .ToList();
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

        public async Task<int?> InsertCartItemQuantityAsync(int userId, string productId, int quantity)
        {
            // Step 1: Get current quantity
            var user = await _userDetail.Find(
                Builders<UserDetail>.Filter.Eq(u => u.UserId, userId)
            ).FirstOrDefaultAsync();

            var item = user?.Cart?.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return null;

       

            // Step 2: Check if the new quantity would be <= 1
            if (quantity <= 0)
            {
                return item.Quantity; // Don't allow reducing to 1 or below
            }

            // Step 3: Proceed with update
            var filter = Builders<UserDetail>.Filter.And(
                Builders<UserDetail>.Filter.Eq(u => u.UserId, userId),
                Builders<UserDetail>.Filter.ElemMatch(u => u.Cart, c => c.ProductId == productId)
            );

            var update = Builders<UserDetail>.Update.Set("Cart.$.Quantity", quantity);/*.Inc("Cart.$.Quantity", changeAmount);*/

            var result = await _userDetail.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return quantity;
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


        public async Task<List<ReceiveInfo>> GetReceiveInfoAsync(int userId)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var projection = Builders<UserDetail>.Projection.Include(u => u.ReceiveInfo).Exclude("_id");
            var result = await _userDetail.Find(filter).Project<UserDetail>(projection).FirstOrDefaultAsync();
            return result?.ReceiveInfo ?? new List<ReceiveInfo>();
        }

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

        public async Task UpdatePhoneNumberAsync(int userId, string newPhoneNumber)
        {
            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Set(u => u.PhoneNumber, newPhoneNumber);
            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task InsertUserCategoriesAsync(int userId, List<string> categoryNames)
        {
            var categoryDict = categoryNames.ToDictionary(name => name, name => 0);

            var filter = Builders<UserDetail>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserDetail>.Update.Set(u => u.Category, categoryDict);

            await _userDetail.UpdateOneAsync(filter, update);
        }

        public async Task UpdateCategoryScoreAsync(int userId, string categoryName, UserAction action)
        {
            int points = action switch
            {
                UserAction.Click => 50,
                UserAction.AddToCart => 100,
                UserAction.Purchase => 200,
                UserAction.ClickCategory => 100,
                _ => 0
            };

            var user = await _userDetail.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null) throw new Exception("User not found");

            //// Initialize Category if null
            //if (user.Category == null)
            //    user.Category = new Dictionary<string, int>();

            // Ensure Reset key exists
            if (!user.Category.ContainsKey("Reset"))
                user.Category["Reset"] = 0;

            // Ensure category key exists
            if (!user.Category.ContainsKey(categoryName))
                user.Category[categoryName] = 0;

            // Update selected category
            user.Category[categoryName] += points;

            // Update Reset
            user.Category["Reset"] += points;

            // --- Check if Reset threshold reached ---
            if (user.Category["Reset"] > 200)
            {
                foreach (var key in user.Category.Keys.ToList())
                {
                    if (key == "Reset") continue;
                    user.Category[key] = (int)Math.Floor(user.Category[key] * 0.8); // decrease by 20%
                }

                user.Category["Reset"] = 0;
            }

            // --- Bonus rule ---
            // Get max score excluding Reset (giúp những tìm kiếm mới được hiện nhiều hơn)
            var maxValue = user.Category
                .Where(kv => kv.Key != "Reset")
                .Select(kv => kv.Value)
                .DefaultIfEmpty(0)
                .Max();

            int current = user.Category[categoryName];

            if (categoryName != "Reset" && current < maxValue /*/ 2*/)
            {
                int bonus = (int)Math.Floor(maxValue * 0.25); // 25% of highest
                user.Category[categoryName] += bonus;
            }

            // Save back to DB
            var update = Builders<UserDetail>.Update.Set(u => u.Category, user.Category);
            await _userDetail.UpdateOneAsync(u => u.UserId == userId, update);
        }

        public async Task<List<UserDetail>> GetAllAsync()
        {
            return await _userDetail.Find(_ => true).ToListAsync();
        }
    }
}
