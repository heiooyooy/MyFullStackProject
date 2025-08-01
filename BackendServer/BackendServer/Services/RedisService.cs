using StackExchange.Redis;

namespace BackendServer.Services;

public enum SeckillResult
{
    Success,
    OutOfStock,
    AlreadyClaimed,
    Error
}

public class RedisService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisService> _logger;
    private readonly LuaScriptProvider _scriptProvider;
    private LoadedLuaScript? _decrementStockScript;

    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger,LuaScriptProvider scriptProvider)
    {
        _database = redis.GetDatabase();
        _logger = logger;
        _scriptProvider = scriptProvider;
    }
    
    public async Task PreheatStockAsync(string productId, int stock)
    {
        var stockKey = $"seckill:product:{productId}:stock";
        var userSetKey = $"seckill:product:{productId}:users";

        await _database.StringSetAsync(stockKey, stock);
        // 清理旧的用户集合，以防万一
        await _database.KeyDeleteAsync(userSetKey);
        _logger.LogInformation("Preheated stock for Product {ProductId} to {Stock} units.", productId, stock);
    }

    public async Task<SeckillResult> ExecuteClaimStockScriptAsync(string productId, string userId)
    {
        var stockKey = $"seckill:product:{productId}:stock";
        var userSetKey = $"seckill:product:{productId}:users";

        try
        {
            var script = _scriptProvider.GetScript("decrement_stock");
            // 定义 Redis keys 和 arguments
            var keys = new RedisKey[] { stockKey, userSetKey};
            var values = new RedisValue[] { userId };
            
            var result = (long)await _database.ScriptEvaluateAsync(
                script.Hash, // 第1个参数: 脚本的SHA1哈希值 (byte[])
                keys, // 第2个参数: KEYS数组，顺序很重要
                values // 第3个参数: ARGV数组，顺序很重要
            );

            return result switch
            {
                1 => SeckillResult.Success,
                0 => SeckillResult.OutOfStock,
                -2 => SeckillResult.AlreadyClaimed,
                _ => SeckillResult.Error
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Lua script for Product {ProductId} and User {UserId}", productId,
                userId);
            return SeckillResult.Error;
        }
    }
}