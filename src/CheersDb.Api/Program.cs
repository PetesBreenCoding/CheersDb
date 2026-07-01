using CheersDb.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetAppSettings();

builder.Services
	.AddAuthentication(appSettings.OpenApiSecurityScheme!.Scheme!)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = appSettings.JwtAuth!.Issuer,
			ValidAudience = appSettings.JwtAuth!.Audience,

			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtAuth!.Key!))
		};
	});

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