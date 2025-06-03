using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Webdev_project.Models
{


    public class UserDetail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int UserId { get; set; }
        public string Avatar { get; set; }

        // e.g., { "Laptop": 2, "Keyboard": 1 }
        public Dictionary<string, int> Category { get; set; }
        public List<CartItem> Cart { get; set; }
        public List<ReceiveInfo> ReceiveInfo { get; set; }

        public string PhoneNumber { get; set; }
        public string Gender { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Birthday { get; set; }

        public Banking Banking { get; set; }
       
        
    }

    public class ReceiveInfo
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class Banking
    {
        public string BankAccount { get; set; }
        public string CreditCard { get; set; }
    }
    //public class CartItem
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string ProductId { get; set; }

    //    public string ProductName { get; set; }
    //    public string Image { get; set; }

    //    public int Quantity { get; set; }
    //    public int UnitPrice { get; set; }
    //}
}
