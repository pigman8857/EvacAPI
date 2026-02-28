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
      var endpoints = _redisDb.Multiplexer.GetEndPoints();
      var server = _redisDb.Multiplexer.GetServer(endpoints.First());
      await server.FlushDatabaseAsync();
    }
  }
}