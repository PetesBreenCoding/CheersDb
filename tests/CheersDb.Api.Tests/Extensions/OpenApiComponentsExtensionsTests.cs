using CheersDb.Api.Extensions;
using CheersDb.Api.Http;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;

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
}