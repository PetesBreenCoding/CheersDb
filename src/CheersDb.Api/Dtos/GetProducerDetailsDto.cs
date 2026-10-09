using CheersDb.Api.Http;

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
	public required int Id { get; init; }

	/// <summary>
	/// The name of the producer
	/// </summary>
	/// <example>Rye River Brewing</example>
	public required string Name { get; init; }

	/// <summary>
	/// The revision number of the producer
	/// </summary>
	/// <example>7</example>
	public required int Revision { get; init; }

	/// <summary>
	/// Links related to the producer, such as a self link to retrieve the producer details
	/// </summary>
	public required List<LinkDto> Links { get; init; }

	/// <summary>
	/// Generates an example of the GetProducerDetailsDto for documentation purposes
	/// </summary>
	/// <returns></returns>
	public static GetProducerDetailsDto GenerateExample()
	{
		return new GetProducerDetailsDto
		{
			Id = 24,
			Name = "Example Producer",
			Revision = 7,
			Links =
			[
				new LinkDto
				{
					Rel = LinkRels.Self,
					Href = "/producers/24",
					Method = HttpMethod.Get.ToString()
				}
			]
		};
	}
}