using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using StackExchange.Redis;
using System.Text.Json;

namespace evacPlanMoni.infras.repositories
{
  public class RedisEvacuationStatusRepository : IEvacuationStatusRepository
  {
    private readonly IDatabase _redisDb;

    public RedisEvacuationStatusRepository(IConnectionMultiplexer redis)
    {
      _redisDb = redis.GetDatabase();
    }

    public async Task<EvacuationStatus?> GetStatusAsync(string zoneId)
    {
      var data = await _redisDb.StringGetAsync($"zone:{zoneId}");
      // Check if it has a value and isn't empty
      if (!data.HasValue || data.IsNullOrEmpty)
      {
        return null;
      }

      // .ToString() definitively converts the RedisValue to a string, 
      // removing all ambiguity for the JsonSerializer.
      return JsonSerializer.Deserialize<EvacuationStatus>(data.ToString());
    }

    public async Task<List<EvacuationStatus>> GetAllStatusesAsync(IEnumerable<string> zoneIds)
    {
      var statuses = new List<EvacuationStatus>();
      foreach (var zoneId in zoneIds)
      {
        var status = await GetStatusAsync(zoneId);
        if (status != null) statuses.Add(status);
      }
      return statuses;
    }

    public async Task SaveStatusAsync(EvacuationStatus status)
    {
      await _redisDb.StringSetAsync($"zone:{status.ZoneId}", JsonSerializer.Serialize(status));
    }

    public async Task ClearAllDatabaseAsync()
    {
      // #### Developer's Note ####
      // When i searhed how to delete all redis data.
      // I found https://oneuptime.com/blog/post/2026-01-25-redis-delete-all-keys-flushall/view
      // but i did not test it yet via code but from CLI which worked perfectly so i assume that it should work on code as well 
      // until after i tested again via http file.
      // It threw an error *This operation is not available unless admin mode is enabled: FLUSHDB'*

      // It seems that FlushDatabaseAsync command needs Admin right because 
      // this is a nuke command so admin right it fail-safe.
      // To make quick fix, usually set allowAdmin=true can be set at Connection string.
      // But this seems ugly to me. I questioned it myself, If it is the right way to do.
      // I am not quite profession with redis since I work with it not so frequent.

      // So, i thought if there are other way around like delete key by its looking or prefix or regex or whatever.
      // which there is an cli command redis-cli KEYS "prefix:*" | xargs redis-cli DEL
      // that question is, can i get all key with prefix?
      // Then there is one.
      var endpoints = _redisDb.Multiplexer.GetEndPoints();
      var server = _redisDb.Multiplexer.GetServer(endpoints.First());

      var keys = server.Keys(pattern: "zone:*").ToArray();

      if (keys.Any())
      {
        await _redisDb.KeyDeleteAsync(keys);
      }
    }
  }
}