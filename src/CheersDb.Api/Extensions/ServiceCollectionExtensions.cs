using Asp.Versioning;
using CheersDb.Api.Settings;
using Microsoft.OpenApi;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the CheersDb API application.
/// </summary>
public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services) 
	{
		/// <summary>
		/// Configures OpenAPI for the application using the provided AppSettings
		/// </summary>
		/// <param name="appSettings"></param>
		public IApiVersioningBuilder AddConfiguredOpenApi(AppSettings appSettings)
		{
			return services.AddApiVersioning()
				.AddApiExplorer(explorerOptions =>
				{
					// Calling "AddApiExplorer" is required for OpenAPI versioning to work correctly.
					// Without this, the generated OpenAPI documents will not be versioned.

					// GroupNameFormat specifies the format of the API version.
					// Without this, versioning will use the literal group names. In our case, that would be 1.0.
					// For compatibility with the "default" /openapi/v1.json behavior from Microsoft.AspNetCore.OpenApi, we use v'VVV' so we can retrieve it using v1.json.
					// See https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings for more information about formatting API versions.
					explorerOptions.GroupNameFormat = "'v'VVV";
				})
				.AddMvc()
				.AddOpenApi(openApiOptions =>
				{
					openApiOptions.Document.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
					openApiOptions.Document.ConfigureDocument(appSettings);
					openApiOptions.Document.ConfigureOperations(appSettings);
				});
		}
	}
}