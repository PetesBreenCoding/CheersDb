using CheersDb.Api.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the CheersDb API application.
/// </summary>
public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services) 
	{
		/// <summary>
		/// Configures authentication using the configured OpenAPI security scheme and JWT bearer authentication.
		/// </summary>
		/// <param name="appSettings">The application settings containing the OpenAPI security scheme.</param>
		/// <returns>An authentication builder configured with JWT bearer authentication.</returns>
		public AuthenticationBuilder AddConfiguredAuthentication(AppSettings appSettings)
		{
			return services.AddAuthentication(appSettings.OpenApi!.Security!.Scheme!).AddJwtBearer();
		}

		/// <summary>
		/// Configures authorization to require authenticated users by default.
		/// </summary>
		/// <remarks>Sets the fallback policy to require an authenticated user.</remarks>
		/// <returns>An authorization builder for configuring authorization services.</returns>
		public AuthorizationBuilder AddConfiguredAuthorization()
		{
			return services.AddAuthorizationBuilder()
				.SetFallbackPolicy(new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build());
		}

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
					options.ConfigureOperations(appSettings);
				});
		}

		/// <summary>
		/// Adds routing with lowercase URLs and query strings.
		/// </summary>
		/// <returns>The service collection with routing configured.</returns>
		public IServiceCollection AddConfiguredRouting()
		{
			return services.AddRouting(options =>
			{
				options.LowercaseUrls = true;
				options.LowercaseQueryStrings = true;
			});
		}

		/// <summary>
		/// Configures HTTP JSON serialization to use strict number handling.
		/// </summary>
		/// <returns>The service collection with strict HTTP JSON number handling configured.</returns>
		public IServiceCollection ConfigureJson()
		{
			return services.ConfigureHttpJsonOptions(options =>
			{
				options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
			});
		}
	}
}