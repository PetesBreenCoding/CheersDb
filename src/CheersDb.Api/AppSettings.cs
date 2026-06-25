using CheersDb.Api.Http;
using Microsoft.OpenApi;

namespace CheersDb.Api;

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
	/// Gets the OpenAPI information for the API, such as title, version, and description.
	/// </summary>
	public OpenApiInfo? OpenApiInfo { get; init; }

	/// <summary>
	/// Gets the OpenAPI security scheme for the API, which defines the authentication and authorization requirements.
	/// </summary>
	public OpenApiSecurityScheme? OpenApiSecurityScheme { get; init; }
}