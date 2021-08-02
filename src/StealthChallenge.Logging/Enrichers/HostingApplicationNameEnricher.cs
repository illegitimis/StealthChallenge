namespace StealthChallenge.Logging
{
    using Serilog.Core;
    using Serilog.Events;

    public class HostingApplicationNameEnricher : ILogEventEnricher
    {
        LogEventProperty _cachedProperty;

        public const string ApplicationNamePropertyName = "app";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
        }

        private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory) =>
            _cachedProperty = _cachedProperty ?? CreateProperty(propertyFactory);

        /// <summary>
        /// Adds a log event property using the friendly name of the application domain
        /// </summary>
        /// <param name="propertyFactory"></param>
        /// <returns></returns>
        private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            var applicationName = System.AppDomain.CurrentDomain.FriendlyName;

            return propertyFactory.CreateProperty(ApplicationNamePropertyName, applicationName);
        }
    }
}
