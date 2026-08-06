namespace CheersDb.Api.Http;

/// <summary>
/// Contains constants for non-standard HTTP header names used in the CheersDb API.
/// </summary>
public static class NonStandardHeaderNames
{
	/// <summary>
	/// The X-RateLimit-Limit header name. Indicates the maximum number of requests allowed in a given time period.
	/// </summary>
	public const string XRateLimitLimit = "X-RateLimit-Limit";

	/// <summary>
	/// The X-RateLimit-Remaining header name. Indicates the number of requests remaining in the current rate limit window.
	/// </summary>
	public const string XRateLimitRemaining = "X-RateLimit-Remaining";

	/// <summary>
	/// The X-RateLimit-Reset header name. Indicates the number of seconds until the rate limit window resets.
	/// </summary>
	public const string XRateLimitReset = "X-RateLimit-Reset";
}