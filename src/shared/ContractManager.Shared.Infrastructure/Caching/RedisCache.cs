using ContractManager.Infrastructure.Caching;
using StackExchange.Redis;

namespace ContractManager.Shared.Infrastructure.Caching;
public class RedisCache : ICacheService
{
    private readonly IDatabase _database;

    public RedisCache(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public virtual async Task Delete(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public virtual async Task<bool> Exists(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public virtual async Task<string> Get(string key)
    {
        return await _database.StringGetAsync(key);
    }

    public virtual async Task Set(string key, string value, TimeSpan ttl)
    {
        await _database.StringSetAsync(key, value, ttl);
    }

    public virtual async Task<long> Increment(string key)
    {
        return await _database.StringIncrementAsync(key, 1);
    }

    public virtual async Task HashSet(string key, HashEntry[] hashEntries, TimeSpan ttl)
    {
        await _database.HashSetAsync(key, hashEntries);
        await _database.KeyExpireAsync(key, ttl);
    }

    public virtual async Task<bool> HashExists(string key, string subKey)
    {
        return await _database.HashExistsAsync(key, subKey, CommandFlags.PreferReplica);
    }

    public virtual async Task<string> HashGet(string key, string subKey)
    {
        return await _database.HashGetAsync(key, subKey, CommandFlags.PreferReplica);
    }

    public virtual async Task SortedSetAdd(string key, long timeRelative, string messageScheduled)
    {
        await _database.SortedSetAddAsync(key, messageScheduled, timeRelative);
    }

    public virtual Task<SortedSetEntry[]> SortedSetRangeByScore(string key, DateTimeOffset scheduledDate)
    {
        var smsMessages = _database.SortedSetRangeByScoreWithScoresAsync(key, scheduledDate.ToUnixTimeMilliseconds(), 0);
        return smsMessages;
    }

    public virtual Task<long> ZremRangeByScore(string key, SortedSetEntry[] valor)
    {
        return _database.SortedSetRemoveRangeByScoreAsync(key, 0, valor[0].Score);
    }

    public virtual Task<long> ZremByScore(string key, double score)
    {
        return _database.SortedSetRemoveRangeByScoreAsync(key, score, score);
    }
}
