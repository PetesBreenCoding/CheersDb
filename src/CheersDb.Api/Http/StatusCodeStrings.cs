namespace CheersDb.Api.Http;

/// <summary>
/// Contains string constants for HTTP status codes used throughout the API.
/// </summary>
public static class StatusCodeStrings
{
	/// <summary>
	/// HTTP status code 200 — OK.
	/// </summary>
	public const string Status200OK = "200";

	/// <summary>
	/// HTTP status code 401 — Unauthorized.
	/// </summary>
	public const string Status401Unauthorized = "401";

	/// <summary>
	/// HTTP status code 403 — Forbidden.
	/// </summary>
	public const string Status403Forbidden = "403";

	/// <summary>
	/// HTTP status code 404 — Not Found.
	/// </summary>
	public const string Status404NotFound = "404";

	/// <summary>
	/// HTTP status code 429 — Too Many Requests.
	/// </summary>
	public const string Status429TooManyRequests = "429";

	/// <summary>
	/// HTTP status code 500 — Internal Server Error.
	/// </summary>
	public const string Status500InternalServerError = "500";
}