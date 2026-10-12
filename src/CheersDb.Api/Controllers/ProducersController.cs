using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
	/// Get producer details
	/// </summary>
	/// <remarks>Finds a specific producer usiung the passed id and returns the details in the response body</remarks>
	/// <param name="id" example="24">The id of the producer to retrieve</param>
	/// <returns>The requested producer details</returns>
	/// <example>GET /producers/24</example>
	[HttpGet("{id:int}", Name = nameof(GetProducerDetails))]
	[ProducesResponseType(typeof(GetProducerDetailsDto), StatusCodes.Status200OK, Description = "Returns the requested producer in the response body")]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Indicates that the request is malformed or contains invalid data", Type = typeof(ProblemDetailsDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Description = "Indicates the requested producer was not found, or the URI is invalid", Type = typeof(ProblemDetailsDto))]
	[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
	public IActionResult GetProducerDetails([FromRoute] int id)
	{
		var producerDetails = new GetProducerDetailsDto()
		{
			Id = id,
			Name = "Rye River Brewing Company",
			Revision = 7,
			Links =
			[
				new()
				{
					Rel = LinkRels.Self,
					Href = Url.RouteUrl(nameof(GetProducerDetails), new { id }) ?? string.Empty,
					Method = HttpMethod.Get.ToString()
				}
			]
		};

		Response.Headers.Append(HeaderNames.ETag, producerDetails.Revision.ToString());

		return Ok(producerDetails);
	}
}