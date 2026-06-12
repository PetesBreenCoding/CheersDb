using Microsoft.OpenApi;

namespace CheersDb.Api;

/// <summary>
/// Represents the application settings for the CheersDb API.
/// </summary>
public class AppSettings
{
	/// <summary>
	/// Gets the OpenAPI information for the API, such as title, version, and description.
	/// </summary>
	public OpenApiInfo? OpenApiInfo { get; init; }
}