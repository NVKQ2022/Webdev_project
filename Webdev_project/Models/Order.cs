using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrderID { get; set; }

    public string UserID { get; set; }  // reference to the user

    public List<OrderItem> Items { get; set; }

    public int TotalAmount { get; set; }  // total cost in smallest unit (e.g., cents)

    public string PaymentMethod { get; set; }  // "Momo", "Cash"

    public string Status { get; set; }  // e.g., "Pending", "Paid", "Shipped", "Delivered", "Cancelled"

    public DateTime CreatedAt { get; set; }

    public string ShippingAddress { get; set; }

    public string PhoneNumber { get; set; }

    
}

public class OrderItem
{
    public string ProductID { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public int UnitPrice { get; set; }  // per item at time of order
}
