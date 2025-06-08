using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace Webdev_project.Models

{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReviewId { get; set; }

        public string ProductId { get; set; }
        public int UserID { get; set; }

        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<string>? MediaURLs { get; set; }

    }
    
}
