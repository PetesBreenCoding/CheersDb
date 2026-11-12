using CheersDb.Api.Dtos;
using CheersDb.Api.Extensions;
using CheersDb.Api.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;

namespace CheersDb.Api.Tests.Extensions;

public class OpenApiComponentsExtensionsTests
{
	[Fact]
	public void ConfigureHeaders_WhenComponentsHeadersAreEmpty_AddsTheExpectedHeaders()
	{
		var components = new OpenApiComponents();

		components.ConfigureHeaders();

		components.Headers.Should().NotBeNull();
		components.Headers.Should().HaveCount(6);

		components.Headers.Should().ContainKey(HeaderNames.CacheControl);
		components.Headers.Should().ContainKey(HeaderNames.ETag);
		components.Headers.Should().ContainKey(HeaderNames.RetryAfter);
		components.Headers.Should().ContainKey(NonStandardHeaderNames.XRateLimitLimit);
		components.Headers.Should().ContainKey(NonStandardHeaderNames.XRateLimitRemaining);
		components.Headers.Should().ContainKey(NonStandardHeaderNames.XRateLimitReset);

		components.Headers[HeaderNames.CacheControl].Description.Should().Be("Instructions for caching mechanisms in responses");
		components.Headers[HeaderNames.CacheControl].Example.Should().NotBeNull();
		components.Headers[HeaderNames.CacheControl].Example!.GetValue<string>().Should().Be("max-age=3600, must-revalidate");
		components.Headers[HeaderNames.CacheControl].Schema.Should().BeSameAs(OpenApiSpec.StringSchema);

		components.Headers[HeaderNames.ETag].Description.Should().Be("Indicates the current version of the resource");
		components.Headers[HeaderNames.ETag].Example.Should().NotBeNull();
		components.Headers[HeaderNames.ETag].Example!.GetValue<int>().Should().Be(453);
		components.Headers[HeaderNames.ETag].Schema.Should().BeSameAs(OpenApiSpec.StringSchema);

		components.Headers[HeaderNames.RetryAfter].Description.Should().Be("Indicates how many seconds the user agent should wait before making a follow-up request");
		components.Headers[HeaderNames.RetryAfter].Example.Should().NotBeNull();
		components.Headers[HeaderNames.RetryAfter].Example!.GetValue<int>().Should().Be(60);
		components.Headers[HeaderNames.RetryAfter].Schema.Should().BeSameAs(OpenApiSpec.StringSchema);

		components.Headers[NonStandardHeaderNames.XRateLimitLimit].Description.Should().Be("Indicates the maximum number of requests that the user is allowed to make in a given amount of time");
		components.Headers[NonStandardHeaderNames.XRateLimitLimit].Example.Should().NotBeNull();
		components.Headers[NonStandardHeaderNames.XRateLimitLimit].Example!.GetValue<int>().Should().Be(1000);
		components.Headers[NonStandardHeaderNames.XRateLimitLimit].Schema.Should().BeSameAs(OpenApiSpec.StringSchema);

		components.Headers[NonStandardHeaderNames.XRateLimitRemaining].Description.Should().Be("Indicates the number of requests remaining in the current rate limit window");
		components.Headers[NonStandardHeaderNames.XRateLimitRemaining].Example.Should().NotBeNull();
		components.Headers[NonStandardHeaderNames.XRateLimitRemaining].Example!.GetValue<int>().Should().Be(999);
		components.Headers[NonStandardHeaderNames.XRateLimitRemaining].Schema.Should().BeSameAs(OpenApiSpec.NumberSchema);

		components.Headers[NonStandardHeaderNames.XRateLimitReset].Description.Should().Be("The number of seconds until the rate limit resets.");
		components.Headers[NonStandardHeaderNames.XRateLimitReset].Example.Should().NotBeNull();
		components.Headers[NonStandardHeaderNames.XRateLimitReset].Example!.GetValue<int>().Should().Be(60);
		components.Headers[NonStandardHeaderNames.XRateLimitReset].Schema.Should().BeSameAs(OpenApiSpec.NumberSchema);
	}

	[Fact]
	public void ConfigureSecuritySchemes_NullScheme_DoesNotInitializeSecuritySchemes()
	{
		var components = new OpenApiComponents();

		components.ConfigureSecuritySchemes(null);

		components.SecuritySchemes.Should().BeNull();
	}

	[Fact]
	public void ConfigureSecuritySchemes_NamedScheme_AddsSchemeUsingItsName()
	{
		var components = new OpenApiComponents();
		var scheme = new OpenApiSecurityScheme { Name = "Custom" };

		components.ConfigureSecuritySchemes(scheme);

		components.SecuritySchemes.Should().ContainSingle()
			.Which.Should().Be(KeyValuePair.Create<string, IOpenApiSecurityScheme>("Custom", scheme));
	}

	[Fact]
	public void ConfigureSecuritySchemes_UnnamedScheme_UsesJwtBearerAuthenticationScheme()
	{
		var components = new OpenApiComponents();
		var scheme = new OpenApiSecurityScheme();

		components.ConfigureSecuritySchemes(scheme);

		components.SecuritySchemes.Should().ContainSingle()
			.Which.Should().Be(KeyValuePair.Create<string, IOpenApiSecurityScheme>(JwtBearerDefaults.AuthenticationScheme, scheme));
	}

	[Fact]
	public async Task ConfigureResponsesAsync_AddsInternalServerErrorResponse()
	{
		OpenApiComponents? configuredComponents = null;
		var builder = WebApplication.CreateBuilder();
		builder.Services.AddOpenApi(options => options.AddDocumentTransformer(async (document, context, cancellationToken) =>
		{
			document.Components ??= new OpenApiComponents();
			var components = document.Components;
			await components.ConfigureResponsesAsync(context, cancellationToken);
			configuredComponents = components;
		}));

		await using var app = builder.Build();
		app.MapOpenApi();
		app.Urls.Add("http://127.0.0.1:0");
		await app.StartAsync(TestContext.Current.CancellationToken);

		var address = app.Services.GetRequiredService<IServer>()
			.Features.Get<IServerAddressesFeature>()!.Addresses.Single();
		using var httpClient = new HttpClient { BaseAddress = new Uri(address) };
		using var documentResponse = await httpClient.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
		documentResponse.EnsureSuccessStatusCode();

		configuredComponents.Should().NotBeNull();
		configuredComponents!.Responses.Should().ContainKey(nameof(HttpStatusCode.InternalServerError));
		var response = configuredComponents.Responses[nameof(HttpStatusCode.InternalServerError)]
			.Should().BeOfType<OpenApiResponse>().Which;
		response.Description.Should().Be(OpenApiSpec.InternalServerErrorDescription);
		response.Content.Should().ContainKey(MediaTypeNames.Application.Json);

		var content = response.Content[MediaTypeNames.Application.Json];
		content.Schema.Should().BeOfType<OpenApiSchemaReference>()
			.Which.Reference.Should().BeOfType<JsonSchemaReference>()
			.Which.ReferenceV3.Should().Be($"#/components/schemas/{nameof(ProblemDetailsDto)}");
		content.Example.Should().NotBeNull();
		content.Example!["type"]!.GetValue<string>().Should().Be("e500");
		content.Example["title"]!.GetValue<string>().Should().Be("Internal Server Error");
		content.Example["status"]!.GetValue<int>().Should().Be((int)HttpStatusCode.InternalServerError);
		content.Example["detail"]!.GetValue<string>().Should().Be("An unexpected internal server error has occurred. Please try again later or contact support if the issue persists.");
	}
}
