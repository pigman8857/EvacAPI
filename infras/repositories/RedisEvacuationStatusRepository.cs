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

    public EvacuationStatus? GetStatus(string zoneId)
    {
      var data = _redisDb.StringGet($"zone:{zoneId}");
      // Check if it has a value and isn't empty
      if (!data.HasValue || data.IsNullOrEmpty)
      {
        return null;
      }

      // .ToString() definitively converts the RedisValue to a string, 
      // removing all ambiguity for the JsonSerializer.
      return JsonSerializer.Deserialize<EvacuationStatus>(data.ToString());
    }

    public List<EvacuationStatus> GetAllStatuses(IEnumerable<string> zoneIds)
    {
      var statuses = new List<EvacuationStatus>();
      foreach (var zoneId in zoneIds)
      {
        var status = GetStatus(zoneId);
        if (status != null) statuses.Add(status);
      }
      return statuses;
    }

    public void SaveStatus(EvacuationStatus status)
    {
      _redisDb.StringSet($"zone:{status.ZoneId}", JsonSerializer.Serialize(status));
    }

    public void ClearAllDatabase()
    {
      var endpoints = _redisDb.Multiplexer.GetEndPoints();
      var server = _redisDb.Multiplexer.GetServer(endpoints.First());
      server.FlushDatabase();
    }
  }
}