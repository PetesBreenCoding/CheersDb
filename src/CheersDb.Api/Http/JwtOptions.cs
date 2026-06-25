namespace CheersDb.Api.Http;

/// <summary>
/// Configuration options for JWT (JSON Web Token) authentication in the CheersDb API.
/// </summary>
public sealed class JwtOptions
{
	/// <summary>
	/// Gets or initializes the issuer of the JWT. This is the principal that issued the token.
	/// </summary>
	public string? Issuer { get; init; }

	/// <summary>
	/// Gets or initializes the intended audience of the JWT. This identifies the recipients that the JWT is intended for.
	/// </summary>
	public string? Audience { get; init; }

	/// <summary>
	/// Gets or initializes the secret key used to sign and verify the JWT.
	/// </summary>
	public string? Key { get; init; }
}