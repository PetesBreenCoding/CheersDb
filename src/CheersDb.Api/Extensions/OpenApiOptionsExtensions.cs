using CheersDb.Api.Controllers;
using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using CheersDb.Api.OpenApi;
using CheersDb.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

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

	private static readonly Dictionary<string, object> _responseExamples =
		new()
		{
			[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status200OK}"] = GetProducerDetailsDto.GenerateExample(),
			[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status400BadRequest}"] = ProblemDetailsDto.GenerateExample(HttpStatusCode.BadRequest),
			[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status404NotFound}"] = ProblemDetailsDto.GenerateExample(HttpStatusCode.NotFound)
		};

	/// <summary>
	/// Adds a document transformer to the OpenAPI options that applies the provided transformation function to the generated OpenAPI document.
	/// </summary>
	/// <param name="options">The OpenAPI options to configure.</param>
	/// <param name="appSettings">The application settings containing the OpenAPI information to be applied to the document.</param>
	public static OpenApiOptions ConfigureDocument(this OpenApiOptions options, AppSettings appSettings)
	{
		return options.AddDocumentTransformer(async (document, context, cancellationToken) =>
		{
			if (appSettings?.OpenApi?.Info is not null)
				document.Info = appSettings.OpenApi.Info;

			document.Security ??= [];
			document.Security.Add(GetDefaultSecurityRequirement(appSettings, document));

			if (appSettings?.OpenApi?.Servers?.Count > 0)
			{
				document.Servers = appSettings.OpenApi.Servers;

				//foreach (var server in document.Servers.Where(srv => !string.IsNullOrEmpty(srv.Url) && !srv.Url.EndsWith(AppSettings.LatestApiVersionUrlAppend)))
				//	server.Url += AppSettings.LatestApiVersionUrlAppend;
			}

			document.Components ??= new OpenApiComponents();

			await document.Components.ConfigureResponsesAsync(context, cancellationToken);
			document.Components.ConfigureHeaders();
			document.Components.ConfigureSecuritySchemes(appSettings?.OpenApi?.Security);
		});
	}

	/// <summary>
	/// Adds an operation transformer to the OpenAPI options that configures operation responses.
	/// </summary>
	public static OpenApiOptions ConfigureOperations(this OpenApiOptions options, AppSettings appSettings)
	{
		return options.AddOperationTransformer(async (operation, context, cancellationToken) =>
		{
			if (operation.Tags?.FirstOrDefault(t => t.Name == "System") != null)
				operation.AddExtension(OpenApiExtensionNames.NonResource, new OpenApiBooleanExtension(true));

			operation.Responses ??= [];
			operation.Responses.Add(StatusCodeStrings.Status401Unauthorized, new OpenApiResponse()
			{
				Description = "Indicates that the user is not authorized to access the resource"
			});

			operation.Responses.Add(StatusCodeStrings.Status403Forbidden, new OpenApiResponse()
			{
				Description = "Indicates that the user is forbidden from accessing the resource"
			});

			operation.Responses.Add(StatusCodeStrings.Status429TooManyRequests, new OpenApiResponse()
			{
				Description = "Indicates that the user has sent too many requests in a given amount of time",
				Headers = new Dictionary<string, IOpenApiHeader>
				{
					[HeaderNames.RetryAfter] = OpenApiSpec.RetryAfterHeaderReference
				}
			});

			operation.Responses.Add(StatusCodeStrings.Status500InternalServerError, OpenApiSpec.InternalServerErrorResponse);

			operation.Responses.TryGetValue(StatusCodeStrings.Status200OK, out var okResponse);

			if (okResponse is not null && okResponse is OpenApiResponse okResponseConcrete)
			{
				okResponseConcrete.Headers ??= new Dictionary<string, IOpenApiHeader>();
				okResponseConcrete.Headers.Add(HeaderNames.CacheControl, OpenApiSpec.CacheControlHeaderReference);
				okResponseConcrete.Headers.Add(HeaderNames.ETag, OpenApiSpec.ETagHeaderReference);
				okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitLimit, OpenApiSpec.RateLimitLimitHeaderReference);
				okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitRemaining, OpenApiSpec.RateLimitRemainingHeaderReference);
				okResponseConcrete.Headers.Add(NonStandardHeaderNames.XRateLimitReset, OpenApiSpec.RateLimitResetHeaderReference);
			}


			foreach (var response in operation.Responses)
			{
				var responseExampleKey = $"{operation.OperationId}{response.Key}";

				if (!string.IsNullOrEmpty(responseExampleKey) && _responseExamples.TryGetValue(responseExampleKey, out object? example))
				{
					var content = response.Value.Content?[MediaTypeNames.Application.Json];
					content?.Examples ??= new Dictionary<string, IOpenApiExample>();
					content?.Examples?.Add(responseExampleKey, new OpenApiExample { Value = JsonSerializer.SerializeToNode(example) });
				}
			}

			operation.Security ??= [];
			operation.Security.Add(GetDefaultSecurityRequirement(appSettings, context.Document));
		});
	}
}