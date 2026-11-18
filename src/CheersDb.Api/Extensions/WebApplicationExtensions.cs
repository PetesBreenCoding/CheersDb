using Scalar.AspNetCore;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring a CheersDb web application.
/// </summary>
public static class WebApplicationExtensions
{
	extension(WebApplication app)
	{
		/// <summary>
		/// Maps development-only OpenAPI and API documentation endpoints.
		/// </summary>
		/// <returns>The web application.</returns>
		public WebApplication MapOpenApiSpec()
		{
			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi().AllowAnonymous();
				app.MapScalarApiReference("/docs").AllowAnonymous();
			}

			return app;
		}
	}
}
