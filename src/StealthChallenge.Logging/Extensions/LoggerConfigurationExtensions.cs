namespace StealthChallenge.Logging
{
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Serilog.Formatting.Compact;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Extension methods for Serilog.LoggerConfiguration.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class LoggerConfigurationExtensions
    {
        internal static readonly IList<LogEvent> ObservedLogEvents = new List<LogEvent>();

        internal static readonly /*ILogEventSink*/CollectionSink CollectionSink = new CollectionSink();

        /// <summary>
        /// Basic Configuration should be used on all environments.
        /// </summary>
        /// <param name="loggerConfiguration"> </param>
        /// <returns> </returns>
        public static LoggerConfiguration BasicConfiguration(
            this LoggerConfiguration loggerConfiguration) =>
            loggerConfiguration
                // The MinimumLevel configuration object provides for one of the log event levels to be specified as the minimum.
                // Log events with level Verbose and higher will be processed and ultimately written to the console.
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Default", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets", LogEventLevel.Debug)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithApplicationName();

        /// <summary>
        /// Writes to a daily rolling file.
        /// </summary>
        /// <param name="loggerConfiguration"> </param>
        /// <param name="filePath"> </param>
        /// <returns> </returns>
        public static LoggerConfiguration RollingFile(
            this LoggerConfiguration loggerConfiguration,
            string filePath = ".log") =>
            loggerConfiguration
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    filePath,
                    rollingInterval: RollingInterval.Day);


        /// <summary>
        /// Writes to a daily rolling file only the events with levels in the [<paramref name="minimum" />,
        /// <paramref name="maximum" />] interval.
        /// Filters log events not in this range.
        /// </summary>
        /// <param name="loggerConfiguration"> </param>
        /// <param name="minimum"> min log level </param>
        /// <param name="maximum"> max log level </param>
        /// <param name="filePath"> </param>
        /// <returns> </returns>
        public static LoggerConfiguration SubLoggerRollingFile(
            this LoggerConfiguration loggerConfiguration,
            LogEventLevel minimum = LogEventLevel.Verbose,
            LogEventLevel maximum = LogEventLevel.Debug,
            string filePath = ".log") =>
            loggerConfiguration
                .WriteTo.Logger(
                    lc => lc
                        .Filter.ByIncludingOnly(logEvent => logEvent.Level >= minimum && logEvent.Level <= maximum)
                        .WriteTo.File(
                            new CompactJsonFormatter(),
                            filePath,
                            rollingInterval: RollingInterval.Day));

        /// <summary>
        /// Adds log events to a concurrent queue.
        /// </summary>
        /// <remarks>
        /// https://github.com/nblumhardt/serilog-timings/blob/master/test/SerilogTimings.Tests/Support/CollectingLogger.cs
        /// Good for testing purposes.
        /// </remarks>
        public static LoggerConfiguration AddCollection(
            this LoggerConfiguration loggerConfiguration,
            ILogEventSink sink = null) =>
            loggerConfiguration
                .MinimumLevel.Is(LevelAlias.Minimum)
                .Enrich.FromLogContext()
                .WriteTo.Sink(sink ?? CollectionSink);
    }
}