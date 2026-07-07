using Microsoft.OpenApi;

namespace CheersDb.Api.Settings;

/// <summary>
/// Represents the OpenAPI settings for the CheersDb API, including information and security scheme.
/// </summary>
public class OpenApiAppSettings
{
	/// <summary>
	/// Gets the OpenAPI information for the API, such as title, version, and description.
	/// </summary>
	public OpenApiInfo? Info { get; init; }

	/// <summary>
	/// Gets the OpenAPI security scheme for the API, which defines the authentication and authorization requirements.
	/// </summary>
	public OpenApiSecurityScheme? Security { get; init; }

	/// <summary>
	/// Gets the list of OpenAPI servers for the API, which defines the available server URLs and descriptions.
	/// </summary>
	public List<OpenApiServer>? Servers { get; init; }
}