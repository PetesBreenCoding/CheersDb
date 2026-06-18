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

	private static readonly Lazy<OpenApiResponseReference> _internalServerErrorResponseReference = new(() => new OpenApiResponseReference(nameof(HttpStatusCode.InternalServerError)));
	private static readonly Lazy<OpenApiHeaderReference> _cacheControlHeaderReference	= new(() => new OpenApiHeaderReference(HeaderNames.CacheControl));
	private static readonly Lazy<OpenApiHeaderReference> _etagHeaderReference = new(() => new OpenApiHeaderReference(HeaderNames.ETag));

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

				document.Components ??= new OpenApiComponents();

				await document.Components.ConfigureResponsesAsync(context, cancellationToken);
				document.Components.ConfigureHeaders();
			});
		}

		/// <summary>
		/// Adds an operation transformer to the OpenAPI options that configures operation responses.
		/// </summary>
		public OpenApiOptions ConfigureOperations()
		{
			return options.AddOperationTransformer(async (operation, context, cancellationToken) =>
			{
				operation.Responses ??= [];
				operation.Responses.Add(_internalServerErrorResponseKey, _internalServerErrorResponseReference.Value);

				operation.Responses.TryGetValue(((int)HttpStatusCode.OK).ToString(), out var okResponse);

				if (okResponse is not null && okResponse is OpenApiResponse okResponseConcrete)
				{
					okResponseConcrete.Headers ??= new Dictionary<string, IOpenApiHeader>();
					okResponseConcrete.Headers.Add(HeaderNames.CacheControl, _cacheControlHeaderReference.Value);
					okResponseConcrete.Headers.Add(HeaderNames.ETag, _etagHeaderReference.Value);
				}
			});
		}
	}
}