namespace CheersDb.Api.Dtos;

/// <summary>
/// Represents the health status of the API
/// </summary>
/// <example>
/// {
///   "status": "Healthy",
///   "timestamp": "2024-01-15T10:30:00Z",
///   "version": "1.0.0.0"
/// }
/// </example>
public class ApiStatusDto
{
	/// <summary>
	/// The health status of the API
	/// </summary>
	/// <example>Healthy</example>
	public required string Status { get; init; }

	/// <summary>
	/// The timestamp when the health check was performed (UTC)
	/// </summary>
	/// <example>2024-01-15T10:30:00Z</example>
	public required DateTime Timestamp { get; init; }

	/// <summary>
	/// The version of the API
	/// </summary>
	/// <example>1.0.0.0</example>
	public required string Version { get; init; }
}
