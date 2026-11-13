using CheersDb.Api.Extensions;
using CheersDb.Api.Http;
using CheersDb.Api.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;
using System.Text.Json;

namespace CheersDb.Api.Tests.Extensions;

public class OpenApiOptionsExtensionsTests
{
	[Fact]
	public async Task ConfigureDocument_AppliesSettingsAndConfiguresDocumentComponents()
	{
		var info = new OpenApiInfo { Title = "Test API", Version = "2.0" };
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
				Security = securityScheme
			}
		};
		OpenApiDocument? configuredDocument = null;

		var builder = WebApplication.CreateBuilder();
		builder.Services.AddOpenApi(options =>
		{
			options.ConfigureDocument(appSettings);
			options.AddDocumentTransformer((document, _, _) =>
			{
				configuredDocument = document;
				return Task.CompletedTask;
			});
		});

		await using var app = builder.Build();
		app.MapOpenApi();
		app.Urls.Add("http://127.0.0.1:0");
		await app.StartAsync(TestContext.Current.CancellationToken);

		var address = app.Services.GetRequiredService<IServer>()
			.Features.Get<IServerAddressesFeature>()!.Addresses.Single();
		using var httpClient = new HttpClient { BaseAddress = new Uri(address) };
		using var response = await httpClient.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
		response.EnsureSuccessStatusCode();
		using var openApiJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

		configuredDocument.Should().NotBeNull();
		configuredDocument!.Info.Should().BeSameAs(info);
		configuredDocument.Servers.Should().ContainSingle().Which.Should().BeSameAs(server);
		configuredDocument.Security.Should().ContainSingle();
		openApiJson.RootElement.GetProperty("security")[0].EnumerateObject().Single().Name.Should().Be("TestAuth");
		configuredDocument.Components.Should().NotBeNull();
		configuredDocument.Components!.SecuritySchemes.Should().ContainKey("TestAuth")
			.WhoseValue.Should().BeSameAs(securityScheme);
		configuredDocument.Components.Responses.Should().ContainKey(nameof(HttpStatusCode.InternalServerError));
		configuredDocument.Components.Headers.Should().ContainKeys(
			HeaderNames.CacheControl,
			HeaderNames.ETag,
			HeaderNames.RetryAfter,
			NonStandardHeaderNames.XRateLimitLimit,
			NonStandardHeaderNames.XRateLimitRemaining,
			NonStandardHeaderNames.XRateLimitReset);
	}
}
