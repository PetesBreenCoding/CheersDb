using CheersDb.Api.OpenApi;
using FluentAssertions;
using Microsoft.OpenApi;

namespace CheersDb.Api.Tests.OpenApi;

public class OpenApiBooleanExtensionTests
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Constructor_SetsValue(bool value)
	{
		var extension = new OpenApiBooleanExtension(value);

		extension.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(true, "true")]
	[InlineData(false, "false")]
	public void Write_WritesBooleanValue(bool value, string expectedJson)
	{
		var extension = new OpenApiBooleanExtension(value);
		using var textWriter = new StringWriter();
		var openApiWriter = new OpenApiJsonWriter(textWriter);

		extension.Write(openApiWriter, OpenApiSpecVersion.OpenApi3_0);
		textWriter.Flush();

		textWriter.ToString().Should().Be(expectedJson);
	}
}