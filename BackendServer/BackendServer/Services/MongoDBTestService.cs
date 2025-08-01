using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SharedModule.MongoDBExp;

namespace BackendServer.Services;

public class MongoDBTestService
{
    private readonly IMongoCollection<MongoDBOrder> _orders;
    private readonly IMongoCollection<MongoDBUser> _users;
    private readonly IMongoCollection<MongoDBProduct> _products;

    public MongoDBTestService(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Database);
        _orders = database.GetCollection<MongoDBOrder>("orders");
        _users = database.GetCollection<MongoDBUser>("users");
        _products = database.GetCollection<MongoDBProduct>("products");
    }

    public async Task<IList<MongoDBOrder>> GetOrders()
    {
        // return await this._orders.AsQueryable().ToListAsync();
        return await this._orders.Find(new BsonDocument()).ToListAsync();
    }


    // --- 方法 1: 获取所有订单 ---
    // 演示: Find, 空过滤器
    // MongoDB 操作: db.orders.find({})
    public async Task<List<MongoDBOrder>> GetAllOrdersAsync()
    {
        // FilterDefinition.Empty 是类型安全、表达清晰的“无条件”过滤器
        return await _orders.Find(FilterDefinition<MongoDBOrder>.Empty).ToListAsync();
    }

    // --- 方法 2: 根据 ID 获取单个用户 ---
    // 演示: Find, 按主键 _id 筛选
    // MongoDB 操作: db.users.find({ "_id": "user001" })
    public async Task<MongoDBUser> GetUserByIdAsync(string userId)
    {
        var filter = Builders<MongoDBUser>.Filter.Eq(u => u.Id, userId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    // --- 方法 3: 使用原生查询获取活跃且年龄大于指定值的用户 ---
    // 演示: 组合过滤器 ($and, $gt, $eq)
    // MongoDB 操作: db.users.find({ status: "active", age: { $gt: 30 } })
    public async Task<List<MongoDBUser>> GetActiveUsersOlderThan_WithBuildersAsync(int age)
    {
        var filterBuilder = Builders<MongoDBUser>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(u => u.Status, "active"),
            filterBuilder.Gt(u => u.Age, age)
        );
        return await _users.Find(filter).ToListAsync();
    }

    // --- 方法 4: 使用 LINQ 实现与方法3相同的功能 ---
    // 演示: LINQ 的 Where
    public async Task<List<MongoDBUser>> GetActiveUsersOlderThan_WithLinqAsync(int age)
    {
        return await _users.AsQueryable()
            .Where(u => u.Status == "active" && u.Age > age)
            .ToListAsync();
    }

    // --- 方法 5: 查询内嵌文档 ---
    // 演示: 查询内嵌文档的字段
    // MongoDB 操作: db.users.find({ "address.city": "Techville" })
    public async Task<List<MongoDBUser>> GetUsersByCityAsync(string city)
    {
        var filter = Builders<MongoDBUser>.Filter.Eq("address.city", city); // 使用字符串路径
        return await _users.Find(filter).ToListAsync();
    }

    // --- 方法 6: 查询数组字段 ---
    // 演示: $in 操作符，查询分类为 "Electronics" 或 "Books" 的产品
    // MongoDB 操作: db.products.find({ category: { $in: ["Electronics", "Books"] } })
    public async Task<List<MongoDBProduct>> GetProductsInCategories_WithBuildersAsync(IEnumerable<string> categories)
    {
        var filter = Builders<MongoDBProduct>.Filter.In(p => p.Category, categories);
        return await _products.Find(filter).ToListAsync();
    }

    // --- 方法 7: 使用 LINQ 实现与方法6相同的功能 ---
    // 演示: LINQ 的 Contains (在 IQueryable 上下文中会被翻译成 $in)
    public async Task<List<MongoDBProduct>> GetProductsInCategories_WithLinqAsync(IEnumerable<string> categories)
    {
        return await _products.AsQueryable()
            .Where(p => categories.Contains(p.Category))
            .ToListAsync();
    }

    // --- 方法 8: 只获取产品名称和价格 (投影) ---
    // 演示: Projection
    // MongoDB 操作: db.products.find({}, { name: 1, price: 1, _id: 0 })
    public async Task<List<BsonDocument>> GetProductNamesAndPricesAsync()
    {
        var projection = Builders<MongoDBProduct>.Projection
            .Include(p => p.Name)
            .Include(p => p.Price)
            .Exclude(p => p.Id);

        // 因为返回的结构不再匹配 MongoDBProduct 类，所以我们将其投影为 BsonDocument
        return await _products.Find(FilterDefinition<MongoDBProduct>.Empty).Project(projection).ToListAsync();
    }

    // --- 方法 9: 使用 LINQ 实现投影到匿名对象 ---
    // 演示: LINQ 的 Select
    public async Task<List<ProductDto>> GetProductDtos_WithLinqAsync()
    {
        return await _products.AsQueryable()
            .Select(p => new ProductDto(p.Name, p.Price))
            .ToListAsync();
    }


    // --- 方法 10: 获取订单并分页、排序 ---
    // 演示: Sort, Skip, Limit
    public async Task<List<MongoDBOrder>> GetOrdersPaginatedAsync(int pageNumber, int pageSize)
    {
        var sort = Builders<MongoDBOrder>.Sort.Descending(o => o.OrderDate);

        return await _orders.Find(FilterDefinition<MongoDBOrder>.Empty)
            .Sort(sort)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }


    // --- 方法 11: 按产品类别统计产品数量 (分组) ---
    // 演示: $group
    // MongoDB 操作: db.products.aggregate([{ $group: { _id: "$category", count: { $sum: 1 } } }])
    public async Task<List<BsonDocument>> GetCategoryCountsAsync()
    {
        // 结果是新的结构，所以用 BsonDocument 接收
        return await _products.Aggregate()
            .Group(
                p => p.Category, // 分组的键
                g => new { Category = g.Key, Count = g.Count() } // 对每个分组进行计算
            )
            .Project(g => new BsonDocument { { "category", g.Category }, { "count", g.Count } }) // 转换为 BsonDocument
            .ToListAsync();
    }


    // --- 方法 12: 获取订单并关联客户信息 (连接) ---
    // 演示: $lookup
    public async Task<List<OrderWithCustomer>> GetOrdersWithCustomerInfoAsync()
    {
        return await _orders.Aggregate()
            .Lookup<MongoDBOrder, MongoDBUser, OrderWithCustomer>(
                _users, // 要关联的外部集合
                order => order.CustomerId, // 本地字段
                user => user.Id, // 外部字段
                result => result.CustomerInfo // 将结果放入新类的这个属性中
            )
            .ToListAsync();
    }


    // --- 方法 13: 计算最畅销的产品 (多阶段聚合) ---
    // 演示: $unwind, $group, $sort, $limit
    public async Task<List<BsonDocument>> GetTopSellingProductsAsync(int topN)
    {
        // 1. 将整个 $group 阶段的逻辑都定义在一个 BsonDocument 中
        var groupStage = new BsonDocument("$group", 
            new BsonDocument {
                { "_id", "$items.product_id" },
                { "totalQuantitySold", new BsonDocument("$sum", "$items.quantity") }
            });
        
        return await _orders.Aggregate()
            // 步骤1：展开 items 数组，让每个 item 成为一个独立的文档
            .Unwind<MongoDBOrder, BsonDocument>(o => o.Items)
            // 步骤2：按产品ID分组，并计算每个产品的总销售数量
            .AppendStage<BsonDocument>(groupStage)
            // 步骤3：按总销售数量降序排列
            .Sort(new BsonDocument("totalQuantitySold", -1))
            // 步骤4：只取前 N 名
            .Limit(topN)
            .ToListAsync();
    }
    
    public async Task<List<TopSellingProductResult>> GetTopSellingProductsAsyncV2(int topN)
    {
        // IAggregateFluent<T> 是类型安全的，每一步都会改变 T 的类型
        var pipeline = _orders.Aggregate()
            // 步骤1：展开 items 数组。
            // Unwind 后，管道中的每个文档代表原始订单中的一个 item。
            // 此时文档结构不再匹配 MongoDBOrder，所以我们让驱动将其视为 BsonDocument。
            .Unwind<MongoDBOrder, BsonDocument>(o => o.Items)

            // 步骤2：按产品ID分组。
            // 这里的 "doc" 是上一步 Unwind 之后产生的 BsonDocument。
            .Group(
                // Key Selector: 定义分组的依据。我们从 BsonDocument 中提取 items.product_id
                doc => doc["items"]["product_id"],

                // Group Projection: 定义分组后的新文档结构。
                // "g" 代表每个分组 (IGrouping<TKey, TElement>)。
                g => new TopSellingProductResult // 投影到一个强类型的 DTO
                {
                    ProductId = g.Key.AsObjectId.ToString(), // g.Key 就是分组的依据
                    // 使用 LINQ 的 Sum()，它会被翻译成 $sum
                    TotalQuantitySold = g.Sum(doc => doc["items"]["quantity"].AsInt32) 
                }
            )

            // 步骤3：按总销售数量降序排列。
            // 此时管道中的文档类型是我们新创建的 TopSellingProductResult。
            .SortByDescending(r => r.TotalQuantitySold)

            // 步骤4：只取前 N 名。
            .Limit(topN);

        return await pipeline.ToListAsync();
    }
}

// --- 基础模型 ---
// MongoDBUser, MongoDBProduct, MongoDBOrder, MongoDBOrderItem
// (此处省略了它们的定义，请参考之前的回答)

// --- 用于接收聚合结果的 DTO (Data Transfer Object) ---
// 这个类用于接收订单及其关联的客户信息
public class OrderWithCustomer
{
    // BsonExtraElements 允许我们捕获所有 Order 的字段，而无需一一列出
    [BsonExtraElements] public BsonDocument OriginalOrder { get; set; }

    // 这个属性将接收 $lookup 的结果
    [BsonElement("customerInfo")] public List<MongoDBUser> CustomerInfo { get; set; }
}

public class TopSellingProductResult
{
    [BsonId] // 对应分组的 _id
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }

    [BsonElement("totalQuantitySold")]
    public int TotalQuantitySold { get; set; }
}

public record ProductDto(string ProductName, double CurrentPrice);

// public record ProductDto
// {
//     public string ProductName { get; init; }
//     public double CurrentPrice { get; init; }
//     public string? Description { get; init; } // 可以有可选属性
// }