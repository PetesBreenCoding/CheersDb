using CheersDb.Api.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;

namespace CheersDb.Api;

/// <summary>
/// Centralized OpenAPI specification constants and references used across the application.
/// </summary>
public static class OpenApiSpec
{
	/// <summary>
	/// Reusable string schema definition.
	/// </summary>
	public static readonly OpenApiSchema StringSchema = new() { Type = JsonSchemaType.String };

	/// <summary>
	/// Reusable number schema definition.
	/// </summary>
	public static readonly OpenApiSchema NumberSchema = new() { Type = JsonSchemaType.Number };

	/// <summary>
	/// Description for the Internal Server Error response
	/// </summary>
	public const string InternalServerErrorDescription = "Indicates that an unexpected internal server error has occurred";

	/// <summary>
	/// A reusable reference to the Internal Server Error response defined in components.Responses.
	/// </summary>
	public static readonly OpenApiResponseReference InternalServerErrorResponse = new(nameof(HttpStatusCode.InternalServerError))
	{
		Description = InternalServerErrorDescription
	};

	/// <summary>
	/// Reusable reference for the Cache-Control response header.
	/// </summary>
	public static readonly OpenApiHeaderReference CacheControlHeaderReference = new(HeaderNames.CacheControl);

	/// <summary>
	/// Reusable reference for the ETag response header.
	/// </summary>
	public static readonly OpenApiHeaderReference ETagHeaderReference = new(HeaderNames.ETag);

	/// <summary>
	/// Reusable reference for the Retry-After response header.
	/// </summary>
	public static readonly OpenApiHeaderReference RetryAfterHeaderReference = new(HeaderNames.RetryAfter);

	/// <summary>
	/// Reusable reference for the X-RateLimit-Limit response header.
	/// </summary>
	public static readonly OpenApiHeaderReference RateLimitLimitHeaderReference = new(NonStandardHeaderNames.XRateLimitLimit);

	/// <summary>
	/// Reusable reference for the X-RateLimit-Remaining response header.
	/// </summary>
	public static readonly OpenApiHeaderReference RateLimitRemainingHeaderReference = new(NonStandardHeaderNames.XRateLimitRemaining);

	/// <summary>
	/// Reusable reference for the X-RateLimit-Reset response header.
	/// </summary>
	public static readonly OpenApiHeaderReference RateLimitResetHeaderReference = new(NonStandardHeaderNames.XRateLimitReset);
}