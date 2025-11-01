using StackExchange.Redis;

namespace ContractManager.Infrastructure.Caching;
public interface ICacheService
{
    Task Delete(string key);
    Task<bool> Exists(string key);
    Task<string> Get(string key);
    Task Set(string key, string value, TimeSpan ttl);
    Task<long> Increment(string key);
    Task HashSet(string key, HashEntry[] hashEntries, TimeSpan ttl);
    Task<bool> HashExists(string key, string subKey);
    Task<string> HashGet(string key, string subKey);
    Task SortedSetAdd(string key, long timeRelative, string taskId);
    Task<SortedSetEntry[]> SortedSetRangeByScore(string key, DateTimeOffset scheduledDate);
    Task<long> ZremRangeByScore(string key, SortedSetEntry[] valor);
    Task<long> ZremByScore(string key, double score);
}
