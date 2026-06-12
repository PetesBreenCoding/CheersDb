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
			var appSettings = configurationManager.Get<AppSettings>();

			if (appSettings is null)
				throw new InvalidOperationException("There was an issue parsing the application settings.");

			if (appSettings.OpenApiInfo is null)
				throw new InvalidOperationException($"{nameof(AppSettings.OpenApiInfo)} was not parsed in the application settings.");

			return appSettings;
		}
	}
}