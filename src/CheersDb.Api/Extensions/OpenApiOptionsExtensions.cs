using CheersDb.Api.Http;
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
	private static readonly string _internalServerErrorResponseKey = ((int)HttpStatusCode.InternalServerError).ToString();
	private static readonly string _tooManyRequestsResponseKey = ((int)HttpStatusCode.TooManyRequests).ToString();

	private static readonly OpenApiResponseReference _internalServerErrorResponseReference = new(nameof(HttpStatusCode.InternalServerError));

	private static readonly OpenApiHeaderReference _cacheControlHeaderReference = new(HeaderNames.CacheControl);
	private static readonly OpenApiHeaderReference _etagHeaderReference = new(HeaderNames.ETag);
	private static readonly OpenApiHeaderReference _retryAfterHeaderReference = new(HeaderNames.RetryAfter);
	private static readonly OpenApiHeaderReference _rateLimitLimitHeaderReference = new(NonStandardHeaderNames.XRateLimitLimit);
	private static readonly OpenApiHeaderReference _rateLimitRemainingHeaderReference = new(NonStandardHeaderNames.XRateLimitRemaining);
	private static readonly OpenApiHeaderReference _rateLimitResetHeaderReference = new(NonStandardHeaderNames.XRateLimitReset);
	private static OpenApiSecurityRequirement? _defaultSecurityRequirement = null;

	private static OpenApiSecurityRequirement BuildDefaultSecurityRequirement(AppSettings? appSettings, OpenApiDocument? document)
	{
		return _defaultSecurityRequirement ??= new OpenApiSecurityRequirement
		{
			{ new OpenApiSecuritySchemeReference(appSettings?.OpenApiSecurityScheme?.Name ?? JwtBearerDefaults.AuthenticationScheme, document), new List<string>() }
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
				if (appSettings?.OpenApiInfo is not null)
					document.Info = appSettings.OpenApiInfo;

				document.Security ??= [];
				document.Security.Add(BuildDefaultSecurityRequirement(appSettings, document));

				document.Components ??= new OpenApiComponents();

				await document.Components.ConfigureResponsesAsync(context, cancellationToken);
				document.Components.ConfigureHeaders();
				document.Components.ConfigureSecuritySchemes(appSettings?.OpenApiSecurityScheme);
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
				operation.Responses.Add(_internalServerErrorResponseKey, _internalServerErrorResponseReference);

				var tooManyRequestsResponse = new OpenApiResponse
				{
					Description = "Indicates that the user has sent too many requests in a given amount of time",
					Headers = new Dictionary<string, IOpenApiHeader>
					{
						[HeaderNames.RetryAfter] = _retryAfterHeaderReference
					}
				};

				operation.Responses.Add(_tooManyRequestsResponseKey, tooManyRequestsResponse);

				operation.Responses.TryGetValue(((int)HttpStatusCode.OK).ToString(), out var okResponse);

				if (okResponse is not null && okResponse is OpenApiResponse okResponseConcrete)
				{
					okResponseConcrete.Headers ??= new Dictionary<string, IOpenApiHeader>();
					okResponseConcrete.Headers.Add(HeaderNames.CacheControl, _cacheControlHeaderReference);
					okResponseConcrete.Headers.Add(HeaderNames.ETag, _etagHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitLimit, _rateLimitLimitHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitRemaining, _rateLimitRemainingHeaderReference);
					okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitReset, _rateLimitResetHeaderReference);
				}

				operation.Security ??= [];
				operation.Security.Add(BuildDefaultSecurityRequirement(appSettings, context.Document));
			});
		}
	}
}