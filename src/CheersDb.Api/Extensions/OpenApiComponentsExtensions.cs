using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI components in the CheersDb API application.
/// </summary>
public static class OpenApiComponentsExtensions
{
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

			var problemDetailsSchema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetailsDto), cancellationToken: cancellationToken);

			components.Responses.Add(nameof(HttpStatusCode.InternalServerError), new OpenApiResponse
			{
				Description = OpenApiSpec.InternalServerErrorDescription,
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[MediaTypeNames.Application.Json] = new OpenApiMediaType
					{
						Schema = new OpenApiSchemaReference(nameof(ProblemDetailsDto)),
						Example = JsonSerializer.SerializeToNode(new ProblemDetailsDto
						{
							Type = "e500",
							Title = "Internal Server Error",
							Status = (int)HttpStatusCode.InternalServerError,
							Detail = "An unexpected internal server error has occurred. Please try again later or contact support if the issue persists."
						}, JsonSerializerOptions.Web)
					}
				}
			});
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
				Example = "max-age=3600, must-revalidate",
				Schema = OpenApiSpec.StringSchema
			};

			var etagHeader = new OpenApiHeader
			{
				Description = "Indicates the current version of the resource",
				Example = 453,
				Schema = OpenApiSpec.StringSchema
			};

			var retryAfterHeader = new OpenApiHeader
			{
				Description = "Indicates how many seconds the user agent should wait before making a follow-up request",
				Example = 60,
				Schema = OpenApiSpec.StringSchema
			};

			var rateLimitLimitHeader = new OpenApiHeader
			{
				Description = "Indicates the maximum number of requests that the user is allowed to make in a given amount of time",
				Example = 1000,
				Schema = OpenApiSpec.StringSchema,
			};

			var rateLimitRemainingHeader = new OpenApiHeader
			{
				Description = "Indicates the number of requests remaining in the current rate limit window",
				Example = 999,
				Schema = OpenApiSpec.NumberSchema,
			};

			var rateLimitResetHeader = new OpenApiHeader
			{
				Description = "The number of seconds until the rate limit resets.",
				Example = 60,
				Schema = OpenApiSpec.NumberSchema,
			};

			components.Headers.Add(HeaderNames.CacheControl, cacheControlHeader);
			components.Headers.Add(HeaderNames.ETag, etagHeader);
			components.Headers.Add(HeaderNames.RetryAfter, retryAfterHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitLimit, rateLimitLimitHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitRemaining, rateLimitRemainingHeader);
			components.Headers.Add(NonStandardHeaderNames.XRateLimitReset, rateLimitResetHeader);
		}

		/// <summary>
		/// Configures the OpenAPI components to include a security scheme for JWT Bearer authentication.
		/// </summary>
		/// <param name="openApiSecurityScheme">The OpenAPI security scheme to include.</param>
		public void ConfigureSecuritySchemes(OpenApiSecurityScheme? openApiSecurityScheme)
		{
			if (openApiSecurityScheme is null)
				return;

			components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
			components.SecuritySchemes.Add(openApiSecurityScheme.Name ?? JwtBearerDefaults.AuthenticationScheme, openApiSecurityScheme);
		}
	}
}