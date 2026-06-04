namespace CheersDb.Api.Dtos;

/// <summary>
/// Represents a hypermedia link in the API response, providing information about the relationship, URL, and HTTP method for the link.
/// </summary>
public class LinkDto
{
	/// <summary>
	/// The relationship of the link to the current resource
	/// </summary>
	/// <example>self</example>
	public required string Rel { get; init; }

	/// <summary>
	/// The URL of the link
	/// </summary>
	/// <example>/producers/1</example>
	public required string Href { get; init; }

	/// <summary>
	/// The HTTP method for the link
	/// </summary>
	/// <example>GET</example>
	public required string Method { get; init; }
}