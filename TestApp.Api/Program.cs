using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using TestApp.Application.Cache;
using TestApp.Application.Providers.One;
using TestApp.Application.Providers.Two;
using TestApp.Application.Services;
using TestApp.Contracts.Caching;
using TestApp.Contracts.Services;
using TestApp.ProviderOneClient;
using TestApp.ProviderTwoClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRouteCache, MemoryRouteCache>();
builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<IProviderOneSearchService, ProviderOneSearchService>();
builder.Services.AddSingleton<IProviderTwoSearchService, ProviderTwoSearchService>();
builder.Services.Configure<CacheConfiguration>
    (builder.Configuration.GetSection("Cache"));
builder.Services.Configure<ProviderOneConfiguration>
    (builder.Configuration.GetSection("ProviderOne"));
builder.Services.Configure<ProviderTwoConfiguration>
    (builder.Configuration.GetSection("ProviderTwo"));
builder.Services.AddProviderOneClient(builder.Configuration);
builder.Services.AddProviderTwoClient(builder.Configuration);
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions                   = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion                   = new ApiVersion(1, 0);
});
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();