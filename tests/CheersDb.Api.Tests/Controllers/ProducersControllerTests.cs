using CheersDb.Api.Controllers;
using CheersDb.Api.Dtos;
using CheersDb.Api.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace CheersDb.Api.Tests.Controllers;

public class ProducersControllerTests
{
	[Fact]
	public void GetProducerDetails_ReturnsProducerDetailsSelfLinkAndRevisionHeader()
	{
		const int producerId = 24;
		var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor());
		var controller = new ProducersController
		{
			ControllerContext = new ControllerContext(actionContext)
		};
		var urlHelper = new FakeUrlHelper(actionContext, "/v1/producers");
		controller.Url = urlHelper;

		var result = controller.GetProducerDetails(producerId);

		result.Should().NotBeNull();
		var response = result.Should().BeOfType<OkObjectResult>().Which;
		response.StatusCode.Should().Be(StatusCodes.Status200OK);
		var details = response.Value.Should().BeOfType<GetProducerDetailsDto>().Which;
		details.Id.Should().Be(producerId);
		details.Name.Should().Be("Rye River Brewing Company");
		details.Revision.Should().Be(7);
		details.Links.Should().ContainSingle().Which.Should().BeEquivalentTo(new LinkDto
		{
			Rel = LinkRels.Self,
			Href = $"/v1/producers/{producerId}",
			Method = HttpMethod.Get.ToString()
		});
		urlHelper.LastRouteContext!.RouteName.Should().Be(nameof(ProducersController.GetProducerDetails));
		new RouteValueDictionary(urlHelper.LastRouteContext.Values)["id"].Should().Be(producerId);
		controller.Response.Headers[HeaderNames.ETag].ToString().Should().Be("7");
	}
}