using CheersDb.Api.Extensions;
using CheersDb.Api.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CheersDb.Api.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
	[Fact]
	public void AddConfiguredAuthentication_ReturnsBuilderForSameServiceCollection()
	{
		var services = new ServiceCollection();
		var appSettings = CreateAppSettings("Bearer");

		var result = services.AddConfiguredAuthentication(appSettings);

		result.Services.Should().BeSameAs(services);
	}

	[Fact]
	public async Task AddConfiguredAuthentication_RegistersConfiguredJwtBearerScheme()
	{
		var services = new ServiceCollection();
		var appSettings = CreateAppSettings("Bearer");
		services.AddLogging();
		services.AddConfiguredAuthentication(appSettings);
		using var provider = services.BuildServiceProvider();

		var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
		var defaultScheme = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
		var bearerScheme = await schemeProvider.GetSchemeAsync("Bearer");

		defaultScheme.Should().NotBeNull();
		defaultScheme!.Name.Should().Be("Bearer");
		bearerScheme.Should().NotBeNull();
		bearerScheme!.HandlerType.Should().Be<JwtBearerHandler>();
	}

	[Fact]
	public void ConfigureJson_ReturnsSameServiceCollection()
	{
		var services = new ServiceCollection();

		var result = services.ConfigureJson();

		result.Should().BeSameAs(services);
	}

	[Fact]
	public void ConfigureJson_ConfiguresStrictNumberHandling()
	{
		var services = new ServiceCollection();
		services.ConfigureJson();
		using var provider = services.BuildServiceProvider();

		var jsonOptions = provider.GetRequiredService<IOptions<JsonOptions>>().Value;

		jsonOptions.SerializerOptions.NumberHandling.Should().Be(JsonNumberHandling.Strict);
	}

	[Fact]
	public void AddConfiguredAuthorization_ReturnsBuilderForSameServiceCollection()
	{
		var services = new ServiceCollection();

		var result = services.AddConfiguredAuthorization();

		result.Services.Should().BeSameAs(services);
	}

	[Fact]
	public void AddConfiguredAuthorization_ConfiguresAuthenticatedUserFallbackPolicy()
	{
		var services = new ServiceCollection();
		services.AddConfiguredAuthorization();
		using var provider = services.BuildServiceProvider();

		var authorizationOptions = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
		var fallbackPolicy = authorizationOptions.FallbackPolicy;

		fallbackPolicy.Should().NotBeNull();
		fallbackPolicy!.Requirements.Should().ContainSingle();
		fallbackPolicy.Requirements.Single().GetType().Name.Should().Be("DenyAnonymousAuthorizationRequirement");
	}

	[Fact]
	public void AddConfiguredRouting_ReturnsSameServiceCollection()
	{
		var services = new ServiceCollection();

		var result = services.AddConfiguredRouting();

		result.Should().BeSameAs(services);
	}

	[Fact]
	public void AddConfiguredRouting_ConfiguresLowercaseUrlsAndQueryStrings()
	{
		var services = new ServiceCollection();
		services.AddConfiguredRouting();
		using var provider = services.BuildServiceProvider();

		var routeOptions = provider.GetRequiredService<IOptions<RouteOptions>>().Value;

		routeOptions.LowercaseUrls.Should().BeTrue();
		routeOptions.LowercaseQueryStrings.Should().BeTrue();
	}

	[Fact]
	public void AddConfiguredOpenApi_ReturnsSameServiceCollection()
	{
		var services = new ServiceCollection();
		var appSettings = new AppSettings();

		var result = services.AddConfiguredOpenApi(appSettings);

		result.Should().BeSameAs(services);
	}

	[Fact]
	public async Task AddConfiguredOpenApi_AppliesDocumentAndOperationConfiguration()
	{
		var info = new OpenApiInfo { Title = "Configured API", Version = "2.0" };
		var server = new OpenApiServer { Url = "https://api.example.test" };
		var securityScheme = new OpenApiSecurityScheme
		{
			Name = "TestAuth",
			Scheme = "bearer",
			Type = SecuritySchemeType.Http
		};
		var appSettings = new AppSettings
		{
			OpenApi = new OpenApiAppSettings
			{
				Info = info,
				Servers = [server],
				Security = securityScheme,
				Tags =
				[
					new OpenApiTag { Name = "System", Description = "System operations" }
				]
			}
		};

		var builder = WebApplication.CreateBuilder();
		var result = builder.Services.AddConfiguredOpenApi(appSettings);
		result.Should().BeSameAs(builder.Services);

		builder.Services.AddRouting();
		await using var app = builder.Build();
		app.MapGet("/health", () => "healthy").WithName("GetHealth").WithTags("System");
		app.MapOpenApi();
		app.Urls.Add("http://127.0.0.1:0");
		await app.StartAsync(TestContext.Current.CancellationToken);

		var address = app.Services.GetRequiredService<IServer>()
			.Features.Get<IServerAddressesFeature>()!.Addresses.Single();
		using var httpClient = new HttpClient { BaseAddress = new Uri(address) };
		using var response = await httpClient.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
		response.EnsureSuccessStatusCode();
		using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
		var root = document.RootElement;

		root.GetProperty("openapi").GetString().Should().StartWith("3.1");
		root.GetProperty("info").GetProperty("title").GetString().Should().Be("Configured API");
		root.GetProperty("info").GetProperty("version").GetString().Should().Be("2.0");
		root.GetProperty("servers")[0].GetProperty("url").GetString().Should().Be("https://api.example.test");

		var operation = root.GetProperty("paths").GetProperty("/health").GetProperty("get");
		operation.GetProperty("x-non-resource").GetBoolean().Should().BeTrue();
		var responses = operation.GetProperty("responses");
		responses.TryGetProperty("200", out _).Should().BeTrue();
		responses.TryGetProperty("401", out _).Should().BeTrue();
		responses.TryGetProperty("403", out _).Should().BeTrue();
		responses.TryGetProperty("429", out _).Should().BeTrue();
		responses.TryGetProperty("500", out _).Should().BeTrue();

		root.GetProperty("security")[0].EnumerateObject().Single().Name.Should().Be("TestAuth");
		root.GetProperty("components").GetProperty("securitySchemes").TryGetProperty("TestAuth", out _).Should().BeTrue();
	}

	private static AppSettings CreateAppSettings(string scheme) => new()
	{
		OpenApi = new OpenApiAppSettings
		{
			Security = new OpenApiSecurityScheme
			{
				Name = scheme,
				Scheme = scheme,
				Type = SecuritySchemeType.Http
			}
		}
	};
}