using CheersDb.Api.Controllers;
using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using CheersDb.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;

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

	// This is hopefully temporary and response examples would be handled better in future .net versions
	private static readonly Dictionary<string, JsonNode> _responseExamples = new()
	{
		[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status200OK}"] = JsonSerializer.SerializeToNode(new GetProducerDetailsDto()
			{
				Id = 24,
				Name = "Rye River Brewing Company",
				Revision = 7,
				Links =
					[
						new LinkDto()
						{
							Rel = LinkRels.Self,
							Href = "/producers/24",
							Method = HttpMethod.Get.ToString()
						}
					]
			})!,
		[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status400BadRequest}"] = JsonSerializer.SerializeToNode(new ProblemDetailsDto()
		{
			Type = "https://example.com/producers/bad-request",
			Title = "Bad Request",
			Detail = "The request is malformed or contains invalid data",
			Status = StatusCodes.Status400BadRequest
		})!,
		[$"{nameof(ProducersController.GetProducerDetails)}{StatusCodes.Status404NotFound}"] = JsonSerializer.SerializeToNode(new ProblemDetailsDto()
		{
			Type = "https://example.com/producers/not-found",
			Title = "Producer Not Found",
			Detail = "The requested producer was not found, or the URI is invalid",
			Status = StatusCodes.Status404NotFound
		})!
	};

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

					if (!string.IsNullOrEmpty(responseExampleKey) && _responseExamples.TryGetValue(responseExampleKey, out JsonNode? example))
					{
						response.Value.Content?[MediaTypeNames.Application.Json]?.Examples ??= new Dictionary<string, IOpenApiExample>();
						response.Value.Content?[MediaTypeNames.Application.Json]?.Examples?.Add(responseExampleKey, new OpenApiExample
						{
							Value = example
						});
					}
				}

				operation.Security ??= [];
				operation.Security.Add(GetDefaultSecurityRequirement(appSettings, context.Document));
			});
		}
	}
}