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
		public IServiceCollection AddConfiguredOpenApi(AppSettings appSettings)
		{
			return services.AddOpenApi(options =>
			{
				options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
				options.ConfigureDocument(appSettings);
				options.ConfigureOperations();
			});
		}
	}
}