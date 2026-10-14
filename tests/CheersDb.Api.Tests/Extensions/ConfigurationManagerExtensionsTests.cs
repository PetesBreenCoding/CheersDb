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

    [Fact]
    public void GetAppSettings_NoConfiguration_ThrowsInvalidOperationException()
    {
        var config = new ConfigurationManager();

        FluentActions.Invoking(() => config.GetAppSettings())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("There was an issue parsing the application settings.");
    }

    [Fact]
    public void GetAppSettings_MissingOpenApi_ThrowsInvalidOperationException()
    {
		var config = BuildConfigManager("appsettings.missingopenapi.json");

		FluentActions.Invoking(() => config.GetAppSettings())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("OpenApi was not parsed in the application settings.");
    }

    [Fact]
    public void GetAppSettings_MissingInfo_ThrowsInvalidOperationException()
    {
		var config = BuildConfigManager("appsettings.missinginfo.json");

		FluentActions.Invoking(() => config.GetAppSettings())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Info was not parsed in the application settings.");
    }

    [Fact]
    public void GetAppSettings_MissingServers_ThrowsInvalidOperationException()
    {
		var config = BuildConfigManager("appsettings.missingservers.json");

        FluentActions.Invoking(() => config.GetAppSettings())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Servers was not parsed in the application settings.");
    }

    [Fact]
    public void GetAppSettings_MissingSecurityNameOrScheme_ThrowsInvalidOperationException()
    {
		var config = BuildConfigManager("appsettings.missingsecurity.json");

        FluentActions.Invoking(() => config.GetAppSettings())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Security was not parsed in the application settings.");
    }

    private static ConfigurationManager BuildConfigManager(string appSettingsFileName)
    {
		var configManager = new ConfigurationManager();
		var path = Path.Combine(AppContext.BaseDirectory, "TestData", appSettingsFileName);
		configManager.AddJsonFile(path, optional: false, reloadOnChange: false);

        return configManager;
	}
}