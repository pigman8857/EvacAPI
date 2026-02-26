namespace evacPlanMoni.infras.configs
{
  public class DatabaseOptions
  {
    public const string SectionName = "ConnectionStrings";
    public string PostgresConnection { get; set; }
    public string RedisConnection { get; set; }
  }
}