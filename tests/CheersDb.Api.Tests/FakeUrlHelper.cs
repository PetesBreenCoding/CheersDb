using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace CheersDb.Api.Tests;

internal sealed class FakeUrlHelper(ActionContext actionContext, string baseUrl) : IUrlHelper
{
	private readonly string _baseUrl = baseUrl;

	public ActionContext ActionContext { get; } = actionContext;

	public UrlRouteContext? LastRouteContext { get; private set; }

	public string? Action(UrlActionContext actionContext) => throw new NotSupportedException();

	public string? Content(string? contentPath) => throw new NotSupportedException();

	public bool IsLocalUrl(string? url) => throw new NotSupportedException();

	public string? Link(string? routeName, object? values) => throw new NotSupportedException();

	public string? RouteUrl(UrlRouteContext routeContext)
	{
		LastRouteContext = routeContext;
		var routeValues = new RouteValueDictionary(routeContext.Values);
		return $"{_baseUrl}/{routeValues["id"]}";
	}
}