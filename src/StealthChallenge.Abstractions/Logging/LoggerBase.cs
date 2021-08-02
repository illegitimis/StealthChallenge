namespace StealthChallenge.Abstractions.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class LoggerBase : ILogger
    {
        public abstract void Debug(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "");

        public abstract void Diagnostic(
            string message,
            object[] values = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        public abstract void Info(string message, params object[] values);
        public abstract void Warn(string message, params object[] values);

        public abstract void Error(Exception exception);
        public abstract void Error(string message, params object[] values);
        public abstract void Fatal(Exception exception);
        public abstract void Fatal(string message, params object[] values);

        public void Write(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Fatal: Fatal(new Exception(message)); break;
                case LogLevel.Error: Error(new Exception(message)); break;
                case LogLevel.Warning: Warn(message); break;
                case LogLevel.Information: Info(message); break;
                case LogLevel.Debug: Debug(message); break;
                default: throw new ArgumentOutOfRangeException(nameof(level), level, nameof(LoggerBase));
            }
        }
    }
}