namespace CheersDb.Api.Dtos;

/// <summary>
/// Details about a producer
/// </summary>
public class GetProducerDetailsDto
{
	/// <summary>
	/// The id of the producer
	/// </summary>
	/// <example>24</example>
	public int? Id { get; init; }

	/// <summary>
	/// The name of the producer
	/// </summary>
	/// <example>Rye River Brewing</example>
	public string? Name { get; init; }

	/// <summary>
	/// The revision number of the producer
	/// </summary>
	public int? Revision { get; init; }

	/// <summary>
	/// Links related to the producer, such as a self link to retrieve the producer details
	/// </summary>
	/// <example>
	///	[
	///		{
	///			"rel": "self",
	///			"href": "/producers/24",
	///			"method": "GET"
	///		}
	///	]	
	/// </example>
	public List<LinkDto>? Links { get; init; }
}