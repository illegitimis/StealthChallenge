namespace StealthChallenge.Abstractions.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides a contract for logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Highest log level supported.
        /// Includes verbose.
        /// Internal system events used for troubleshooting.
        /// </summary>
        /// <param name="message">Debug message template.</param>
        /// <param name="values">Named properties values.</param>
        /// <param name="memberName"></param>
        void Debug(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "");

        void Diagnostic(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Insight into application workflows.
        /// Level higher than warning, lower than debug.
        /// </summary>
        /// <param name="message">Operational intelligence message.</param>
        /// <param name="values">Named properties values.</param>
        void Info(string message, params object[] values);

        /// <summary>
        /// Service is degraded or endangered messages.
        /// Level higher error, lower than info.
        /// </summary>
        /// <param name="message">Warning message.</param>
        /// <param name="values"> </param>
        void Warn(string message, params object[] values);

        /// <summary>
        /// Overload error call for exceptions.
        /// Level higher fatal, lower than warning.
        /// Use when functionality is unavailable, invariants are broken or data is lost.
        /// </summary>
        /// <param name="exception"></param>
        void Error(Exception exception);
        void Error(string message, params object[] values);

        /// <summary>
        /// Deal breaker.
        /// Overload call for exceptions.
        /// </summary>
        /// <param name="exception"></param>
        void Fatal(Exception exception);
        void Fatal(string message, params object[] values);

        void Write(LogLevel level, string message);

        /// <summary>
        /// Logs information level message with duration of action. Elapsed property.
        /// </summary>
        /// <param name="messageTemplate"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IDisposable Time(string messageTemplate, params object[] args);

        IDisposable Benchmark(
          BenchmarkType benchmarkType,
          string inputType,
          string deviceId,
          Guid messageId,
          int port);
    }
}
