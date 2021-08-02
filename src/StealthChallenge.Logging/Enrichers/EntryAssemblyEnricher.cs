namespace StealthChallenge.Logging.Mvc
{
    using Serilog.Core;
    using Serilog.Events;
    using System.Reflection;

    public class EntryAssemblyEnricher : ILogEventEnricher
    {
        LogEventProperty _cachedProperty;

        public const string EntryAssemblyPropertyName = "assembly";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
        }

        private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory) =>
            _cachedProperty = _cachedProperty ?? CreateProperty(propertyFactory);

        private LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory) =>
            propertyFactory.CreateProperty(
                EntryAssemblyPropertyName,
                GetEntryAssemblyPropertyValue(Assembly.GetEntryAssembly().GetName()));

        private string GetEntryAssemblyPropertyValue(AssemblyName assemblyName) =>
            $"{assemblyName.Name} {assemblyName.Version}";
    }
}
