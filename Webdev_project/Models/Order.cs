using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


public class Order
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrderID { get; set; }  // Maps to "_id"

    public string UserID { get; set; }

    public List<OrderItem> Items { get; set; }

    public int TotalAmount { get; set; }

    public string PaymentMethod { get; set; }

    public string Status { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public string ShippingAddress { get; set; }

    public string PhoneNumber { get; set; }

}

public class OrderItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductID { get; set; }

    public string ProductName { get; set; }

    public string Image { get; set; }

    public int Quantity { get; set; }

    public int UnitPrice { get; set; }
}
