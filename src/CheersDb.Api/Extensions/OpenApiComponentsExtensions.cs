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
	private static readonly Lazy<OpenApiSchema> _stringSchema = new(() => new OpenApiSchema { Type = JsonSchemaType.String });

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

			var response = new OpenApiResponse
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

			components.Responses.Add(nameof(HttpStatusCode.InternalServerError), response);
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
				Schema = _stringSchema.Value
			};

			var etagHeader = new OpenApiHeader
			{
				Description = "Indicates the current version of the resource",
				Schema = _stringSchema.Value
			};

			components.Headers.Add(HeaderNames.CacheControl, cacheControlHeader);
			components.Headers.Add(HeaderNames.ETag, etagHeader);
		}
	}
}
