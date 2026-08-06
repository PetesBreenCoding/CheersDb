using CheersDb.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CheersDb.Api.Controllers;

/// <summary>
/// Controller for health status checks in the CheersDb API.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SystemController : ControllerBase
{
	/// <summary>
	/// Get the health status of the API
	/// </summary>
	/// <remarks>Returns the current health status of the API and service information</remarks>
	/// <returns>The health status of the API</returns>
	[HttpGet("status", Name = nameof(GetStatus))]
	[ProducesResponseType(typeof(ApiStatusDto), StatusCodes.Status200OK, Description = "Indicates the API is healthy and operational")]
	public IActionResult GetStatus()
	{
		var healthStatus = new ApiStatusDto()
		{
			Status = "Healthy",
			Timestamp = DateTime.UtcNow,
			Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown"
		};

		return Ok(healthStatus);
	}
}