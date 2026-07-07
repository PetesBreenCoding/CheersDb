using CheersDb.Api.Http;
using CheersDb.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI options in the CheersDb API application.
/// </summary>
public static class OpenApiOptionsExtensions
{
	private static OpenApiSecurityRequirement? _defaultSecurityRequirement = null;
	private static OpenApiSecurityRequirement GetDefaultSecurityRequirement(AppSettings? appSettings, OpenApiDocument? document)
	{
		return _defaultSecurityRequirement ??= new OpenApiSecurityRequirement
		{
			{ new OpenApiSecuritySchemeReference(appSettings?.OpenApi?.Security?.Name ?? JwtBearerDefaults.AuthenticationScheme, document), new List<string>() }
		};
	}

	extension(OpenApiOptions options)
	{
		/// <summary>
		/// Adds a document transformer to the OpenAPI options that applies the provided transformation function to the generated OpenAPI document.
		/// </summary>
		/// <param name="appSettings">The application settings containing the OpenAPI information to be applied to the document.</param>
		public OpenApiOptions ConfigureDocument(AppSettings appSettings)
		{
			return options.AddDocumentTransformer(async (document, context, cancellationToken) =>
			{
				if (appSettings?.OpenApi?.Info is not null)
					document.Info = appSettings.OpenApi.Info;

				document.Security ??= [];
				document.Security.Add(GetDefaultSecurityRequirement(appSettings, document));

				if (appSettings?.OpenApi?.Servers?.Count > 0)
					document.Servers = appSettings.OpenApi.Servers;

				document.Components ??= new OpenApiComponents();

				await document.Components.ConfigureResponsesAsync(context, cancellationToken);
				document.Components.ConfigureHeaders();
				document.Components.ConfigureSecuritySchemes(appSettings?.OpenApi?.Security);
			});
		}

		/// <summary>
		/// Adds an operation transformer to the OpenAPI options that configures operation responses.
		/// </summary>
		public OpenApiOptions ConfigureOperations(AppSettings appSettings)
		{
			return options.AddOperationTransformer(async (operation, context, cancellationToken) =>
			{
				operation.Responses ??= [];
				
				operation.Responses.Add(OpenApiSpec.UnauthorizedResponseKey, new OpenApiResponse()
				{
					Description = "Indicates that the user is not authorized to access the resource"
				});

				operation.Responses.Add(OpenApiSpec.TooManyRequestsResponseKey, new OpenApiResponse()
				{
					Description = "Indicates that the user has sent too many requests in a given amount of time",
					Headers = new Dictionary<string, IOpenApiHeader>
					{
						[HeaderNames.RetryAfter] = OpenApiSpec.RetryAfterHeaderReference
					}
				});
				
				operation.Responses.Add(OpenApiSpec.InternalServerErrorResponseKey, OpenApiSpec.InternalServerErrorResponse);

				operation.Responses.TryGetValue(((int)HttpStatusCode.OK).ToString(), out var okResponse);

				if (okResponse is not null && okResponse is OpenApiResponse okResponseConcrete)
				{
					okResponseConcrete.Headers ??= new Dictionary<string, IOpenApiHeader>();
					okResponseConcrete.Headers.Add(HeaderNames.CacheControl, OpenApiSpec.CacheControlHeaderReference);
					okResponseConcrete.Headers.Add(HeaderNames.ETag, OpenApiSpec.ETagHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitLimit, OpenApiSpec.RateLimitLimitHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitRemaining, OpenApiSpec.RateLimitRemainingHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitReset, OpenApiSpec.RateLimitResetHeaderReference);
				}

				operation.Security ??= [];
				operation.Security.Add(GetDefaultSecurityRequirement(appSettings, context.Document));
			});
		}
	}
}