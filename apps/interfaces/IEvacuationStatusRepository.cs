

using evacPlanMoni.entities;

namespace evacPlanMoni.apps.interfaces
{
  public interface IEvacuationStatusRepository
  {
    EvacuationStatus? GetStatus(string zoneId);
    List<EvacuationStatus> GetAllStatuses(IEnumerable<string> zoneIds);
    void SaveStatus(EvacuationStatus status);
    void ClearAllDatabase();
  }
}