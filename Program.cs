using jgz_evenemangel.Services;
using jgz_evenemangel.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);
RegisterMongoDb(builder);
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();
builder.Services.AddSingleton<ApiKeyService>();
builder.Services.AddSingleton<EventService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors();
app.UseOutputCache();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
return;

static void RegisterMongoDb(WebApplicationBuilder builder)
{
    builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.Section));
    builder.Services.AddSingleton<IMongoClient>(s =>
    {
        var settings = s.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        return new MongoClient(settings.ConnectionString);
    });
    builder.Services.AddSingleton<IMongoDatabase>(s =>
    {
        var client = s.GetRequiredService<IMongoClient>();
        var settings = s.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        return client.GetDatabase(settings.DatabaseName);
    });
}