using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedModule.MongoDBExp;

// --- 基础模型类 (对应数据库集合) ---

// 对应 "users" 集合
// --- 对应 "users" 集合 ---
public class MongoDBUser
{
    [BsonId] // 主键
    [BsonRepresentation(BsonType.String)] // 在C#中用string类型，对应数据库的自定义string _id
    public string Id { get; set; }

    [BsonElement("user_name")] public string UserName { get; set; }

    [BsonElement("email")] public string Email { get; set; }

    // **已添加 Age 字段**
    [BsonElement("age")] public int Age { get; set; }

    // **已添加 Status 字段**
    [BsonElement("status")] public string Status { get; set; }

    [BsonElement("registered_date")] public DateTime RegisteredDate { get; set; }

    // **已添加 Address 内嵌文档对应的类**
    [BsonElement("address")] public MongoDBAddress Address { get; set; }
}

public class MongoDBAddress
{
    [BsonElement("street")] public string Street { get; set; }

    [BsonElement("city")] public string City { get; set; }

    [BsonElement("country")] public string Country { get; set; }
}

// --- 对应 "products" 集合 ---
public class MongoDBProduct
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("category")] public string Category { get; set; }

    [BsonElement("price")] public double Price { get; set; }

    [BsonElement("stock")] public int Stock { get; set; }

    [BsonElement("tags")] public List<string> Tags { get; set; }

    [BsonElement("specs")] public List<MongoDBSpec> Specs { get; set; }
}

public class MongoDBSpec
{
    [BsonElement("k")] public string K { get; set; }
    [BsonElement("v")] public string V { get; set; }
}

// --- 对应 "orders" 集合 ---
public class MongoDBOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("customer_id")] public string CustomerId { get; set; }

    [BsonElement("order_date")] public DateTime OrderDate { get; set; }

    [BsonElement("status")] public string Status { get; set; }

    [BsonElement("items")] public List<MongoDBOrderItem> Items { get; set; }
}

public class MongoDBOrderItem
{
    [BsonElement("product_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }

    [BsonElement("quantity")] public int Quantity { get; set; }

    [BsonElement("price_at_purchase")] public double PriceAtPurchase { get; set; }
}

// --- 关键！定义用于接收聚合结果的新模型类 ---

public class MongoDBOrderWithDetails
{
    // 包含 Order 的所有原始字段
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("customer_id")] public string CustomerId { get; set; }

    [BsonElement("order_date")] public DateTime OrderDate { get; set; }

    [BsonElement("status")] public string Status { get; set; }

    [BsonElement("items")] public List<MongoDBOrderItem> Items { get; set; }

    // **新增的字段，用于接收 $lookup 的结果**
    // 对应 $lookup 中 "as": "customerInfo"
    [BsonElement("customerInfo")] public List<MongoDBUser> CustomerInfo { get; set; } // $lookup 的结果总是一个数组

    // 对应 $lookup 中 "as": "productDetails"
    [BsonElement("productDetails")] public List<MongoDBProduct> ProductDetails { get; set; } // 同样是数组
}