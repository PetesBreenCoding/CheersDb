using Microsoft.OpenApi;

namespace CheersDb.Api.OpenApi;

/// <summary>
/// Represents a custom OpenAPI extension for boolean values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenApiBooleanExtension"/> class with the specified boolean value.
/// </remarks>
/// <param name="value">The boolean value for the extension.</param>
public class OpenApiBooleanExtension(bool value) : IOpenApiExtension
{
	/// <summary>
	/// Gets the boolean value of the extension.
	/// </summary>
	public bool Value { get; } = value;

	/// <summary>
	/// Writes the boolean value of the extension to the specified <see cref="IOpenApiWriter"/>.
	/// </summary>
	/// <param name="writer">The OpenAPI writer.</param>
	/// <param name="specVersion">The OpenAPI specification version.</param>
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		writer.WriteValue(Value);
	}
}