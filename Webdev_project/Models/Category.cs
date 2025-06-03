using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Webdev_project.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        public string CateName { get; set; }

        public string Image {  get; set; }
        public int BuyTime { get; set; }

        public int ProductQuantity { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Products { get; set; }
    }

   
}
