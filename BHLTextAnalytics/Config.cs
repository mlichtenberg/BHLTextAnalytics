using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BHLTextAnalytics
{
    public static class Config
    {
        public static string SubscriptionKey { get; set; }
        public static string Endpoint { get; set; }
        public static string BhlApiKey { get; set; }
        public static string InputFolder { get; set; }
        public static string OutputFolder { get; set; }

        static Config()
        {
            // Load the appropriate config file, based on the ASPNETCORE_ENVIRONMENT environment variable setting
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string configFileName = "appsettings.json";
            if (!string.IsNullOrWhiteSpace(envName))
            {
                configFileName = string.Format($"appsettings.{envName}.json");
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFileName, optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            // Read the settings from the config file
            var appSettings = configuration.GetSection("appSettings");
            SubscriptionKey = appSettings.GetSection("textAnalyticsSubscriptionKey").Value;
            Endpoint = appSettings.GetSection("textAnalyticsEndpoint").Value;
            BhlApiKey = appSettings.GetSection("bhlApiKey").Value;
            InputFolder = appSettings.GetSection("inputFolder").Value;
            OutputFolder = appSettings.GetSection("outputFolder").Value;
        }
    }
}
