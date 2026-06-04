using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheersDb.Api.Controllers;

/// <summary>
/// Controller for managing producers in the CheersDb API.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProducersController : ControllerBase
{
	/// <summary>
	/// Retrieves details for a specific producer by id
	/// </summary>
	/// <param name="id">The id of the producer to retrieve</param>
	/// <returns>The requested producer details</returns>
	/// <response code="200">Returns the requested producer in the response body</response>
	/// <response code="404">Indicates the requested producer was not found, or the URI is invalid</response>
	[HttpGet("{id:int}", Name = nameof(GetProducerDetails))]
	[ProducesResponseType(typeof(GetProducerDetailsDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult GetProducerDetails([FromRoute] int id)
	{
		return Ok(new GetProducerDetailsDto()
		{
			Id = id,
			Name = "Producer Name",
			Links =
			[
				new() 
				{
					Rel = LinkRelationships.Self,
					Href = Url.RouteUrl(nameof(GetProducerDetails), new { id }) ?? string.Empty,
					Method = HttpMethod.Get.ToString()
				}
			]
		});
	}
}