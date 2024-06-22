using Microsoft.Extensions.Configuration;

namespace LibSSH.AppSettings
{
    /// <summary>
    /// Provides access to configuration values stored in an appsettings.json file.
    /// </summary>
    public static class AppSettingsReader
    {
        private static IConfigurationRoot _configuration;

        /// <summary>
        /// Initializes the configuration by loading the appsettings.json file.
        /// </summary>
        static AppSettingsReader()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = configBuilder.Build();
        }

        /// <summary>
        /// Retrieves a configuration value based on the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration value to retrieve.</param>
        /// <returns>The configuration value associated with the specified key, or null if the key does not exist.</returns>
        public static string? GetConfigurationValue(string key)
        {
            return _configuration[key];
        }
    }
}
