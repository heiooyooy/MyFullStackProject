namespace SharedModule;

public record SeckillClaimSucceededEvent(
    string EventId, 
    string ProductId, 
    string UserId, 
    DateTime TimestampUtc,
    string? UserAgent, // 可以加入更多丰富的分析性数据
    string? IpAddress
);