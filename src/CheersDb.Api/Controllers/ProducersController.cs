using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CheersDb.Api.Controllers;

/// <summary>
/// Controller for managing producers in the CheersDb API.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ProducersController : ControllerBase
{
	/// <summary>
	/// Retrieves details for a specific producer by id
	/// </summary>
	/// <param name="id">The id of the producer to retrieve</param>
	/// <returns>The requested producer details</returns>
	[HttpGet("{id:int}", Name = nameof(GetProducerDetails))]
	[ProducesResponseType(typeof(GetProducerDetailsDto), StatusCodes.Status200OK, Description = "Returns the requested producer in the response body")]
	[ProducesResponseType(StatusCodes.Status404NotFound, Description = "Indicates the requested producer was not found, or the URI is invalid")]
	[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
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
					Rel = LinkRels.Self,
					Href = Url.RouteUrl(nameof(GetProducerDetails), new { id }) ?? string.Empty,
					Method = HttpMethod.Get.ToString()
				}
			]
		});
	}
}