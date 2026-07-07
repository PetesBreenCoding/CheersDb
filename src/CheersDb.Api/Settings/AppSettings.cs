using CheersDb.Api.Http;

namespace CheersDb.Api.Settings;

/// <summary>
/// Represents the application settings for the CheersDb API.
/// </summary>
public class AppSettings
{
	/// <summary>
	/// Gets the JWT auth settings
	/// </summary>
	public JwtOptions? JwtAuth { get; init; }

	/// <summary>
	/// Gets the OpenAPI settings for the API, including information and security scheme.
	/// </summary>
	public OpenApiAppSettings? OpenApi { get; init; }
}