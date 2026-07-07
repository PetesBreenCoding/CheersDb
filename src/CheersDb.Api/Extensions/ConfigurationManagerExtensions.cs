using CheersDb.Api.Settings;

namespace CheersDb.Api.Extensions;

/// <summary>
/// Provides extension methods for the ConfigurationManager class
/// </summary>
public static class ConfigurationManagerExtensions
{
	extension(ConfigurationManager configurationManager)
	{
		/// <summary>
		/// Gets the application settings from the configuration manager and validates them
		/// </summary>
		/// <returns>A validated AppSettings instance.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the application settings are invalid.</exception>
		public AppSettings GetAppSettings()
		{
			var appSettings = configurationManager.Get<AppSettings>() 
							?? throw new InvalidOperationException("There was an issue parsing the application settings.");

			if (appSettings.OpenApi is null)
				throw new InvalidOperationException($"{nameof(AppSettings.OpenApi)} was not parsed in the application settings.");

			if (appSettings.OpenApi.Info is null)
				throw new InvalidOperationException($"{nameof(AppSettings.OpenApi.Info)} was not parsed in the application settings.");
			
			if (appSettings.OpenApi.Servers?.Count is null or 0)
				throw new InvalidOperationException($"{nameof(AppSettings.OpenApi.Servers)} was not parsed in the application settings.");

			if (string.IsNullOrEmpty(appSettings.OpenApi.Security?.Name) || string.IsNullOrEmpty(appSettings.OpenApi.Security?.Scheme))
				throw new InvalidOperationException($"{nameof(AppSettings.OpenApi.Security)} was not parsed in the application settings.");

			if (string.IsNullOrEmpty(appSettings.JwtAuth?.Key))
				throw new InvalidOperationException($"{nameof(AppSettings.JwtAuth)} was not parsed in the application settings.");

			return appSettings;
		}
	}
}