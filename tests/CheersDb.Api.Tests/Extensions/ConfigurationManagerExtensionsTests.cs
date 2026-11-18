using CheersDb.Api.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace CheersDb.Api.Tests.Extensions;

public class ConfigurationManagerExtensionsTests
{
    [Fact]
    public void GetAppSettings_ValidConfiguration_ReturnsAppSettings()
    {
        var config = BuildConfigManager("appsettings.valid.json");

        var appSettings = config.GetAppSettings();

        appSettings.Should().NotBeNull();
        appSettings.OpenApi.Should().NotBeNull();
        appSettings.OpenApi!.Info!.Title.Should().Be("Cheers");
        appSettings.OpenApi.Servers.Should().HaveCount(1);
        appSettings.OpenApi.Security!.Name.Should().Be("Auth");
    }

    [Theory]
    [InlineData(null, "There was an issue parsing the application settings.")]
    [InlineData("appsettings.missingopenapi.json", "OpenApi was not parsed in the application settings.")]
	[InlineData("appsettings.missinginfo.json", "Info was not parsed in the application settings.")]
	[InlineData("appsettings.missingservers.json", "Servers was not parsed in the application settings.")]
	[InlineData("appsettings.missingsecurity.json", "Security was not parsed in the application settings.")]
    public void GetAppSettings_InvalidConfiguration_ThrowsInvalidOperationException(string? appSettingsFileName, string expectedMessage)
	{
		var config = BuildConfigManager(appSettingsFileName);

		FluentActions.Invoking(() => config.GetAppSettings())
			.Should().Throw<InvalidOperationException>()
			.WithMessage(expectedMessage);
	}

    private static ConfigurationManager BuildConfigManager(string? appSettingsFileName)
    {
		var configManager = new ConfigurationManager();

        if (string.IsNullOrEmpty(appSettingsFileName))
			return configManager;

		var path = Path.Combine(AppContext.BaseDirectory, "TestData", appSettingsFileName);
		configManager.AddJsonFile(path, optional: false, reloadOnChange: false);

        return configManager;
	}
}