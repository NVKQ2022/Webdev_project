using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Webdev_project.Models;

public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Để MongoDB tự sinh

    public int UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
