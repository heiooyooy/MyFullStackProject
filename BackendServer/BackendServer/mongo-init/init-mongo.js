// 切换到 'test' 数据库。如果不存在，MongoDB 会在第一次插入数据时创建它。
db = db.getSiblingDB('test');

// --- 用户 (users) 集合 (已更新) ---
db.users.drop();

db.users.insertMany([
    {
        "_id": "user001",
        "user_name": "Alice",
        "email": "alice@example.com",
        "age": 30, // **数据**
        "status": "active", // **数据**
        "registered_date": ISODate("2023-01-15T09:00:00Z"),
        "address": {
            "street": "123 Main St",
            "city": "Techville",
            "country": "USA"
        }
    },
    {
        "_id": "user002",
        "user_name": "Bob",
        "email": "bob@example.com",
        "age": 25, // **数据**
        "status": "active", // **数据**
        "registered_date": ISODate("2023-03-20T14:30:00Z"),
        "address": {
            "street": "456 Oak Ave",
            "city": "Datatown",
            "country": "Canada"
        }
    },
    {
        "_id": "user003",
        "user_name": "Charlie",
        "email": "charlie@example.com",
        "age": 35, // **数据**
        "status": "inactive", // **数据**
        "registered_date": ISODate("2022-11-10T18:00:00Z"),
        "address": {
            "street": "789 Pine Ln",
            "city": "Techville",
            "country": "USA"
        }
    },
    {
        "_id": "user004",
        "user_name": "Dejun",
        "email": "dejun@example.com",
        "age": 35, // **数据**
        "status": "active", // **数据**
        "registered_date": ISODate("2022-11-10T18:00:00Z"),
        "address": {
            "street": "789 Pine Ln",
            "city": "Techville",
            "country": "USA"
        }
    }
]);

// --- 产品 (products) 集合 (与之前相同) ---
db.products.drop();

db.products.insertMany([
    {
        "_id": ObjectId("60d21b4667d0d8992e610c85"),
        "name": "Laptop Pro 15-inch",
        "category": "Electronics",
        "price": 1299.99,
        "stock": 50,
        "tags": ["powerful", "professional", "laptop"],
        "specs": [{"k": "Screen", "v": "15-inch Retina"}, {"k": "CPU", "v": "M3 Pro"}, {"k": "RAM", "v": "16GB"}]
    },
    {
        "_id": ObjectId("60d21b4667d0d8992e610c86"),
        "name": "Wireless Mouse",
        "category": "Electronics",
        "price": 49.99,
        "stock": 200,
        "tags": ["ergonomic", "wireless", "accessory"]
    },
    {
        "_id": ObjectId("60d21b4667d0d8992e610c87"),
        "name": "The Go Programming Language",
        "category": "Books",
        "price": 39.99,
        "stock": 150,
        "tags": ["programming", "google", "book"]
    },
    {
        "_id": ObjectId("60d21b4667d0d8992e610c88"),
        "name": "Men's Cotton T-Shirt",
        "category": "Clothing",
        "price": 25.00,
        "stock": 300,
        "tags": ["cotton", "apparel", "men"]
    }
]);

// --- 订单 (orders) 集合 (与之前相同) ---
db.orders.drop();

db.orders.insertMany([
    {
        "customer_id": "user001",
        "order_date": ISODate("2024-05-10T10:00:00Z"),
        "status": "shipped",
        "items": [
            {"product_id": ObjectId("60d21b4667d0d8992e610c85"), "quantity": 1, "price_at_purchase": 1299.99},
            {"product_id": ObjectId("60d21b4667d0d8992e610c86"), "quantity": 1, "price_at_purchase": 49.99}
        ]
    },
    {
        "customer_id": "user002",
        "order_date": ISODate("2024-06-20T11:30:00Z"),
        "status": "pending",
        "items": [{"product_id": ObjectId("60d21b4667d0d8992e610c87"), "quantity": 2, "price_at_purchase": 39.99}]
    },
    {
        "customer_id": "user001",
        "order_date": ISODate("2024-07-15T15:00:00Z"),
        "status": "shipped",
        "items": [
            {"product_id": ObjectId("60d21b4667d0d8992e610c88"), "quantity": 5, "price_at_purchase": 25.00},
            {"product_id": ObjectId("60d21b4667d0d8992e610c87"), "quantity": 1, "price_at_purchase": 45.99}
        ]
    },
    {
        "customer_id": "user003",
        "order_date": ISODate("2024-07-22T16:00:00Z"),
        "status": "cancelled",
        "items": [{"product_id": ObjectId("60d21b4667d0d8992e610c86"), "quantity": 1, "price_at_purchase": 49.99}]
    }
]);

print("Corrected sample data for 'users', 'products', and 'orders' collections created successfully!");
