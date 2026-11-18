using CheersDb.Api.Extensions;
using CheersDb.Api.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CheersDb.Api.Tests.Extensions;

public class WebApplicationExtensionsTests
{
	[Fact]
	public async Task MapOpenApiSpec_DevelopmentMapsAnonymousDocumentationEndpoints()
	{
		var builder = WebApplication.CreateBuilder();
		builder.Environment.EnvironmentName = Environments.Development;
		builder.Services.AddConfiguredOpenApi(new AppSettings());
		await using var app = builder.Build();

		var result = app.MapOpenApiSpec();

		result.Should().BeSameAs(app);
		var routeEndpoints = ((Microsoft.AspNetCore.Routing.IEndpointRouteBuilder)app).DataSources
			.SelectMany(source => source.Endpoints)
			.OfType<RouteEndpoint>()
			.ToList();
		var routePatterns = routeEndpoints
			.Select(endpoint => endpoint.RoutePattern.RawText)
			.Where(pattern => pattern is not null)
			.Cast<string>()
			.ToList();
		routePatterns.Should().Contain(pattern => pattern.Contains("openapi", StringComparison.OrdinalIgnoreCase));
		routePatterns.Should().Contain(pattern => pattern.Contains("docs", StringComparison.OrdinalIgnoreCase));
		routeEndpoints.Where(endpoint =>
			endpoint.RoutePattern.RawText?.Contains("openapi", StringComparison.OrdinalIgnoreCase) == true ||
			endpoint.RoutePattern.RawText?.Contains("docs", StringComparison.OrdinalIgnoreCase) == true)
			.Should().OnlyContain(endpoint => endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null);
	}

	[Fact]
	public async Task MapOpenApiSpec_ProductionMapsNoDevelopmentEndpoints()
	{
		var builder = WebApplication.CreateBuilder();
		builder.Environment.EnvironmentName = Environments.Production;
		await using var app = builder.Build();

		var result = app.MapOpenApiSpec();

		result.Should().BeSameAs(app);
		var routeEndpoints = ((Microsoft.AspNetCore.Routing.IEndpointRouteBuilder)app).DataSources
			.SelectMany(source => source.Endpoints)
			.OfType<RouteEndpoint>()
			.ToList();
		var routePatterns = routeEndpoints
			.Select(endpoint => endpoint.RoutePattern.RawText)
			.Where(pattern => pattern is not null)
			.Cast<string>()
			.ToList();
		routePatterns.Should().NotContain(pattern => pattern.Contains("openapi", StringComparison.OrdinalIgnoreCase));
		routePatterns.Should().NotContain(pattern => pattern.Contains("docs", StringComparison.OrdinalIgnoreCase));
	}
}
