using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Webdev_project.Models;


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
    public ReceiveInfo ReceiveInfo { get; set; }

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

public class ReceiveInfo
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}
