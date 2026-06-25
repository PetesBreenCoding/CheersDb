using CheersDb.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetAppSettings();

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = "cheersdb-api",
			ValidAudience = "cheersdb-clients",

			IssuerSigningKey =
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345")) //TODO add proper key 
		};
	});

// Add services to the container.
builder.Services.AddControllers();

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
	app.MapOpenApi();
	app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();