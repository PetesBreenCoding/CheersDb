using CheersDb.Api.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI components in the CheersDb API application.
/// </summary>
public static class OpenApiComponentsExtensions
{
	private static readonly OpenApiSchema _stringSchema = new() { Type = JsonSchemaType.String };
	private static readonly OpenApiSchema _numberSchema = new() { Type = JsonSchemaType.Number };

	extension(OpenApiComponents components)
	{
		/// <summary>
		/// Adds a response to the OpenAPI components if it does not already exist.
		/// </summary>
		/// <param name="context">The context of the OpenAPI document transformation.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
		public async Task ConfigureResponsesAsync(OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
		{
			components.Responses ??= new Dictionary<string, IOpenApiResponse>();

			var problemDetailsSchema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), cancellationToken: cancellationToken);

			var internalServerErrorResonse = new OpenApiResponse
			{
				Description = "Indicates that an unexpected internal server error has occurred",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[MediaTypeNames.Application.Json] = new OpenApiMediaType
					{
						Schema = new OpenApiSchemaReference(nameof(ProblemDetails))
					}
				}
			};

			components.Responses.Add(nameof(HttpStatusCode.InternalServerError), internalServerErrorResonse);
		}

		/// <summary>
		/// Configures the OpenAPI components to include a header for caching mechanisms in responses.
		/// </summary>
		public void ConfigureHeaders()
		{
			components.Headers ??= new Dictionary<string, IOpenApiHeader>();

			var cacheControlHeader = new OpenApiHeader
			{
				Description = "Instructions for caching mechanisms in responses",
				Schema = _stringSchema
			};

			var etagHeader = new OpenApiHeader
			{
				Description = "Indicates the current version of the resource",
				Schema = _stringSchema
			};

			var retryAfterHeader = new OpenApiHeader
			{
				Description = "Indicates how many seconds the user agent should wait before making a follow-up request",
				Schema = _numberSchema
			};

			var rateLimitLimitHeader = new OpenApiHeader
			{
				Description = "Indicates the maximum number of requests that the user is allowed to make in a given amount of time",
				Example = 1000,
				Schema = _numberSchema,
			};

			var rateLimitRemainingHeader = new OpenApiHeader
			{
				Description = "Indicates the number of requests remaining in the current rate limit window",
				Example = 999,
				Schema = _numberSchema,
			};

			var rateLimitResetHeader = new OpenApiHeader
			{
				Description = "The number of seconds until the rate limit resets.",
				Example = 60,
				Schema = _numberSchema,
			};

			components.Headers.Add(HeaderNames.CacheControl, cacheControlHeader);
			components.Headers.Add(HeaderNames.ETag, etagHeader);
			components.Headers.Add(HeaderNames.RetryAfter, retryAfterHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitLimit, rateLimitLimitHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitRemaining, rateLimitRemainingHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitReset, rateLimitResetHeader);
		}
	}
}
