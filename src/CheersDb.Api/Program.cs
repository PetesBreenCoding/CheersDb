using CheersDb.Api.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetAppSettings();

builder.Services.AddAuthentication(appSettings.OpenApi!.Security!.Scheme!)
				.AddJwtBearer();

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});

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