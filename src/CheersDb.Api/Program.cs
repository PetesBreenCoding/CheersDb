using CheersDb.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Scalar.AspNetCore;
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

builder.Services.AddAuthorizationBuilder()
	.SetFallbackPolicy(new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build());


builder.Services.AddRouting(options =>
{
	options.LowercaseUrls = true; 
	options.LowercaseQueryStrings = true;
});

builder.Services.AddConfiguredOpenApi(appSettings);

builder.Services.AddSingleton(appSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi().AllowAnonymous();
	app.MapScalarApiReference("/docs").AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();