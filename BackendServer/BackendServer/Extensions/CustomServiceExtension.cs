using System.Text;
using System.Threading.Channels;
using BackendServer.Interfaces;
using BackendServer.Services;
using Elastic.Clients.Elasticsearch;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedModule;
using SharedModule.Interfaces;
using SharedModule.RabbitMQ;
using SharedModule.Services;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.MongoDBExp;

namespace BackendServer.Extensions;

public static class CustomServiceExtension
{
    public static void AddCustomServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        // 注册 MediatR 服务
        // 这会自动扫描指定的程序集（这里是当前项目）中所有的请求处理器和通知处理器
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
        );
        services.AddOpenApi();
        services.AddControllers();


        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMq"));
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<MongoDBSettings>(configuration.GetSection("MongoDB"));
        
        // 2. 配置 MassTransit (连接到 RabbitMQ)
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));
            });
        });

        // 2. 配置并注册 Elasticsearch 客户端
        var esUri = configuration["Elasticsearch:Uri"];
        if (string.IsNullOrEmpty(esUri))
        {
            throw new Exception("Elasticsearch URI is not configured.");
        }
        var settings = new ElasticsearchClientSettings(new Uri(esUri)).DefaultIndex("orders"); // 可以设置一个默认的索引名称
        var client = new ElasticsearchClient(settings);
        // 将客户端注册为单例服务
        services.AddSingleton(client);


        string redisConfiguration = configuration.GetConnectionString("RedisConnection");
        Console.WriteLine(redisConfiguration);
        // // 使用 Redis 缓存作为 Session 的存储
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = redisConfiguration; // Redis 地址
        //     options.InstanceName = "MyAppSession:"; // 键名前缀
        // });
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));

        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"))
        );

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "my-api", // Must match your token's issuer
                    ValidAudience = "my-clients", // Must match your token's audience
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6mWi2glc0/bOEZGLEJdQoEQqQioEusMlj/GXKceLCuc=")),
                    ClockSkew = TimeSpan.Zero
                };
            });


        // Then add authorization
        services.AddAuthorization(options =>
        {
            // Set the default policy to use JWT Bearer
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MyDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<IOrderService, OrderService>();


        // services.AddHostedService<OutboxProcessor>();


        var channel = Channel.CreateUnbounded<Guid>();
        services.AddSingleton(channel.Writer);
        services.AddSingleton(channel.Reader);

        // ======================= 这是需要添加/修改的部分 =======================
        // 3. 创建一个无边界的 Channel 作为单例
        var channelSeckill = Channel.CreateUnbounded<SeckillClaimSucceededEvent>();
        // 4. 将 Channel 的写入端注册为单例，供 Controller 使用
        services.AddSingleton(channelSeckill.Writer);
        // 5. 将 Channel 的读取端注册为单例，供 BackgroundService 使用 (这行代码解决了您的错误)
        services.AddSingleton(channelSeckill.Reader);

        services.AddSingleton<RedisService>();
        services.AddHostedService<OrderCreationService>();

        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<SseNotificationService>();
        services.AddSingleton<MongoDBTestService>();
        
        // 添加 FluentValidation 服务
        // 这会自动查找程序集中所有的 AbstractValidator<T> 并注册它们
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>(); // 指定从哪个程序集加载验证器
    }
}