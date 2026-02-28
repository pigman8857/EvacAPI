
using evacPlanMoni.infras.configs;
using evacPlanMoni.infras.dbcontexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace evacPlanMoni.infras.extentions
{
  public static class InfrastructureServiceCollectionExtensions
  {
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
      // 1. Bind the configuration options
      services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

      // 2. Configure Redis Connection (Singleton)
      services.AddSingleton<IConnectionMultiplexer>(sp =>
      {
        // We resolve the options to get the connection string safely
        var options = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        return ConnectionMultiplexer.Connect(options.RedisConnection);
      });

      // 3. Configure PostgreSQL Connection (Scoped via EF Core)
      // Assuming an ApplicationDbContext for Data Access Layer
      services.AddDbContext<EvacuationDbContext>((sp, options) =>
      {
        var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        options.UseNpgsql(dbOptions.PostgresConnection);
      });

      return services;
    }
  }
}