using CheersDb.Api.Http;

namespace CheersDb.Api.Settings;

/// <summary>
/// Represents the application settings for the CheersDb API.
/// </summary>
public class AppSettings
{
	/// <summary>
	/// Gets the latest API version supported by the application.
	/// </summary>
	public const string LatestApiVersionForUrls = "v1";

	/// <summary>
	/// Gets the OpenAPI settings for the API, including information and security scheme.
	/// </summary>
	public OpenApiAppSettings? OpenApi { get; init; }
}