using System.Reflection;
using StackExchange.Redis;

namespace BackendServer.Services;

public class LuaScriptProvider
{
    private readonly IDatabase _database;

    private readonly Dictionary<string, LoadedLuaScript> _scripts = new();

    public LuaScriptProvider(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task LoadAllScriptsAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        var multiplexer = _database.Multiplexer;
        var endpoint = multiplexer.GetEndPoints().FirstOrDefault();
        if (endpoint == null)
        {
            Console.WriteLine("错误：未找到 Redis 服务器端点。");
            return;
        }
    
        // 获取该端点对应的 IServer 实例
        var server = multiplexer.GetServer(endpoint);

        var scriptNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".lua") && name.Contains("LuaScripts"));

        foreach (var name in scriptNames)
        {
            using var stream = assembly.GetManifestResourceStream(name);
            using var reader = new StreamReader(stream);
            var scriptContent = await reader.ReadToEndAsync();
            
            var preparedScript = LuaScript.Prepare(scriptContent);

            var loadedScript = await preparedScript.LoadAsync(server);
        
            var scriptKey = Path.GetFileNameWithoutExtension(name).Split('.').Last();
            _scripts.Add(scriptKey, loadedScript);
        
            Console.WriteLine($"已加载 Lua 脚本 '{scriptKey}' 到服务器 {server.EndPoint}，SHA1: {BitConverter.ToString(loadedScript.Hash).Replace("-", "")}");
        }
    }
    
    public LoadedLuaScript GetScript(string name)
    {
        if (_scripts.TryGetValue(name, out var script))
        {
            return script;
        }
        throw new KeyNotFoundException($"The Lua script '{name}' was not found or loaded.");
    }
}