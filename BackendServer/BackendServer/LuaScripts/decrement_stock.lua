---
--- 秒杀库存扣减原子脚本
---
--- KEYS[1]: 库存数的 key (e.g., 'seckill:product:123:stock')
--- KEYS[2]: 已抢购用户集合的 key (e.g., 'seckill:product:123:users')
--- ARGV[1]: 当前请求的用户ID (e.g., 'user-abc')
---
--- 返回值:
--- 1: 抢购成功
--- 0: 库存不足
--- -2: 用户已抢购过，禁止重复购买
---

-- 检查用户是否已经是集合的成员
if redis.call('SISMEMBER', KEYS[2], ARGV[1]) == 1 then
    return -2
end

-- 获取当前库存，并转换为数字
local stock = tonumber(redis.call('GET', KEYS[1]))

-- 检查库存是否存在且大于0
if stock and stock > 0 then
    -- 原子性地将库存减1
    redis.call('DECR', KEYS[1])
    -- 将用户ID添加到集合中
    redis.call('SADD', KEYS[2], ARGV[1])
    return 1
else
    return 0
end