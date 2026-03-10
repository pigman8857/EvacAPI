using evacPlanMoni.apps.interfaces;
using evacPlanMoni.apps.Services;
using evacPlanMoni.infras.extentions;
using evacPlanMoni.infras.mappers;
using evacPlanMoni.infras.repositories;
using evacPlanMoni.presentations.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(EvacuationProfile));

// Use the Infrastructure Extension Method ---
builder.Services.AddInfrastructureServices(builder.Configuration);

// Scoped means a new instance of the repository is created per HTTP request.
builder.Services.AddScoped<IEvacuationDataRepository, PostgresEvacuationDataRepository>();
builder.Services.AddScoped<IEvacuationStatusRepository, RedisEvacuationStatusRepository>();
// Add services to the container.
builder.Services.AddScoped<IEvacuationService, EvacuationService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.InitializeInfrastructure();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
