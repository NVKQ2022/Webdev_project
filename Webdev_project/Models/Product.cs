using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Webdev_project.Models
{
    public class Product
    {
        [BsonId] // Marks this property as MongoDB's _id
        [BsonRepresentation(BsonType.ObjectId)] // Allows mapping ObjectId to string
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Price { get; set; }
        public List<string> Color { get; set; }
        public Dictionary<string, int > Rating { get; set; }
        public List<string> ImageURL { get; set; }
        public Dictionary<string, string> Detail { get; set; }
        public string Category { get; set; }
        public int Sold { get; set; }
    }
}
