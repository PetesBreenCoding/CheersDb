using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI options in the CheersDb API application.
/// </summary>
public static class OpenApiOptionsExtensions
{
	private static readonly string _internalServerErrorResponseKey = ((int)HttpStatusCode.InternalServerError).ToString();
	private static readonly Lazy<OpenApiResponseReference> _internalServerErrorResponseReference = new(() => new OpenApiResponseReference(nameof(HttpStatusCode.InternalServerError)));

	/// <summary>
	/// Adds a document transformer to the OpenAPI options that applies the provided transformation function to the generated OpenAPI document.
	/// </summary>
	/// <param name="options">The OpenAPI options to configure.</param>
	/// <param name="appSettings">The application settings containing the OpenAPI information to be applied to the document.</param>
	public static OpenApiOptions ConfigureDocument(this OpenApiOptions options, AppSettings appSettings)
	{
		return options.AddDocumentTransformer(async (document, context, cancellationToken) =>
		{
			if (appSettings?.OpenApiInfo is not null)
				document.Info = appSettings.OpenApiInfo;

			document.Components ??= new OpenApiComponents();
			document.Components.Responses ??= new Dictionary<string, IOpenApiResponse>();

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

			document.Components.Responses.Add(nameof(HttpStatusCode.InternalServerError), response);
		});
	}

	/// <summary>
	/// Adds an operation transformer to the OpenAPI options that configures operation responses.
	/// </summary>
	/// <param name="options">The OpenAPI options to configure.</param>
	/// <param name="appSettings">The application settings.</param>
	public static OpenApiOptions ConfigureOperations(this OpenApiOptions options, AppSettings appSettings)
	{
		return options.AddOperationTransformer(async (operation, context, cancellationToken) =>
		{
			operation.Responses ??= [];
			operation.Responses.Add(_internalServerErrorResponseKey, _internalServerErrorResponseReference.Value);
		});
	}
}