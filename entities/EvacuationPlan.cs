namespace evacPlanMoni.entities;

public class EvacuationPlan
{
    public string ZoneId { get; set; }
    public string VehicleId { get; set; }
    public double ETAHours { get; set; }
    public int PeopleToEvacuate { get; set; }
}