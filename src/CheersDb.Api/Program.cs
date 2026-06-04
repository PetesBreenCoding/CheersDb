using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRouting(options =>
{
	options.LowercaseUrls = true; 
	options.LowercaseQueryStrings = true;
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
