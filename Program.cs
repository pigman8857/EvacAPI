using evacPlanMoni.apps.interfaces;
using evacPlanMoni.apps.Services;
using evacPlanMoni.infras.repositories;
using evacPlanMoni.presentation.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


//  Register Redis Connection as a Singleton
// StackExchange.Redis is designed to share a single connection multiplexer across the whole app.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// Scoped means a new instance of the repository is created per HTTP request.
builder.Services.AddScoped<IEvacuationStatusRepository, RedisEvacuationStatusRepository>();

// Add services to the container.
builder.Services.AddScoped<IEvacuationService, EvacuationService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
