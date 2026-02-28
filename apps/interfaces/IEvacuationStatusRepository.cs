

using evacPlanMoni.entities;

namespace evacPlanMoni.apps.interfaces
{
  public interface IEvacuationStatusRepository
  {
    Task<EvacuationStatus?> GetStatusAsync(string zoneId);
    Task<List<EvacuationStatus>> GetAllStatusesAsync(IEnumerable<string> zoneIds);
    Task SaveStatusAsync(EvacuationStatus status);
    Task ClearAllDatabaseAsync();
  }
}