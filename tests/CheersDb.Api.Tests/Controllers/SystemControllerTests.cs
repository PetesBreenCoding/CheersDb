using CheersDb.Api.Controllers;
using CheersDb.Api.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheersDb.Api.Tests.Controllers;

public class SystemControllerTests
{
	[Fact]
	public void GetStatus_ReturnsHealthyStatusWithTimestampAndVersion()
	{
		var requestStartedAt = DateTime.UtcNow;
		var controller = new SystemController();

		var result = controller.GetStatus();

		var response = result.Should().BeOfType<OkObjectResult>().Which;
		response.StatusCode.Should().Be(StatusCodes.Status200OK);
		var status = response.Value.Should().BeOfType<ApiStatusDto>().Which;
		status.Status.Should().Be("Healthy");
		status.Timestamp.Kind.Should().Be(DateTimeKind.Utc);
		status.Timestamp.Should().BeOnOrAfter(requestStartedAt).And.BeOnOrBefore(DateTime.UtcNow);
		status.Version.Should().Be(typeof(SystemController).Assembly.GetName().Version?.ToString() ?? "Unknown");
	}
}
