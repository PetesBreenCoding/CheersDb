using CheersDb.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetAppSettings();

builder.Services.AddConfiguredAuthentication(appSettings);

builder.Services.ConfigureJson();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddConfiguredAuthorization();

builder.Services.AddConfiguredRouting();

builder.Services.AddConfiguredOpenApi(appSettings);

builder.Services.AddSingleton(appSettings);

var app = builder.Build();

app.MapOpenApiSpec();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();