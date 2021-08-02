namespace StealthChallenge.Logging
{
    using Serilog;
    using Serilog.Configuration;
    using System;

    public static class LoggerEnrichmentConfigurationExtensions
    {
        /// <summary>
        /// Enrich log events with a ApplicationName property containing the current <see cref="System.AppDomain.CurrentDomain.FriendlyName"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithApplicationName(
           this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<HostingApplicationNameEnricher>();
        }
    }
}
