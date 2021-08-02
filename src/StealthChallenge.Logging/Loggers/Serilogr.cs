namespace StealthChallenge.Logging.Loggers
{
    using StealthChallenge.Abstractions.Extensions;
    using StealthChallenge.Abstractions.Logging;
    using Serilog;
    using Serilog.Debugging;
    using Serilog.Events;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using static PropertyNames;
    using ISerilog = Serilog.ILogger;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Unit tests")]
    public class Serilogr : LoggerBase
    {
        protected const string DateFormat = "yyyyMMdd";

        /// <summary>
        /// adapted Serilog instance
        /// </summary>
        protected ISerilog Log;

        /// <summary>
        /// ctor creates logger unless provided.
        /// </summary>
        /// <remarks>
        /// <see cref="https://github.com/serilog/serilog-formatting-compact"/> for CLEF files.
        /// 
        /// Add a console sink:
        /// const string TimestampFormat = "hh:mm:ss.fff";
        /// const string GlobalMessageTemplate = "{Timestamp:" + TimestampFormat + "} [s:{ClassName}.{MemberName}] [{Level:u11}] {Message:lj}{NewLine}";
        /// .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: GlobalMessageTemplate)
        /// </remarks>
        /// <param name="log"></param>
        public Serilogr(ISerilog log = null)
        {
            Log = log ?? (new LoggerConfiguration()).AddCollection().CreateLogger();
            SelfLog.Disable();
        }

        public override void Debug(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "")
        {
            if (!IsDebug) return;

            Log
                .ForContext(MemberName, memberName)
                .Debug(message, values);
        }

        /// <summary>
        /// Most verbose logging we've got.
        /// </summary>
        /// <param name="message">message template</param>
        /// <param name="values">named properties</param>
        /// <param name="memberName">Even if not provided explicitly, will get the method name.</param>
        /// <param name="sourceFilePath">File path of file that contains the caller</param>
        /// <param name="sourceLineNumber">Line number at which method called.</param>
        public override void Diagnostic(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!IsDebug) return;

            Log
                .ForContext(MemberName, memberName)
                .ForContext(FilePath, sourceFilePath)
                .ForContext(LineNumber, sourceLineNumber)
                .Debug(message, values);
        }

        public override void Info(string message, params object[] values)
        {
            if (!IsInfo) return;
            Log.Information(message, values);
        }

        public override void Warn(string message, params object[] values)
        {
            if (!IsWarn) return;
            Log.Warning(message, values);
        }

        public override void Error(Exception exception)
        {
            if (!IsError) return;
            Log.Error(exception.Reduce(), string.Empty);
        }

        public override void Error(string message, params object[] values)
        {
            if (!IsError) return;
            Log.Error(message, values);
        }

        public override void Fatal(Exception exception) =>
            Log.Fatal(exception.Reduce(), string.Empty);

        public override void Fatal(string message, params object[] values) => Log.Fatal(message, values);

        protected bool IsDebug =>
            Log.IsEnabled(LogEventLevel.Debug) || Log.IsEnabled(LogEventLevel.Verbose);

        protected bool IsInfo =>
            Log.IsEnabled(LogEventLevel.Information);

        protected bool IsWarn =>
            Log.IsEnabled(LogEventLevel.Warning);

        protected bool IsError =>
            Log.IsEnabled(LogEventLevel.Error) || Log.IsEnabled(LogEventLevel.Fatal);
    }
}
