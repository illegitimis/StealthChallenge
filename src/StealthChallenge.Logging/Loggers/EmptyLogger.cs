namespace StealthChallenge.Logging.Loggers
{
    using StealthChallenge.Abstractions.Infrastructure;
    using StealthChallenge.Abstractions.Logging;
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Null object pattern for generic <see cref="ILogger{T}" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyLogger<T> : ILogger<T>
    {
        public void Debug(string message, object[] values = null, [CallerMemberName] string memberName = "") => EmptyLog();

        public void Diagnostic(string message, object[] values = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) => EmptyLog();


        public void Error(Exception exception) => EmptyLog();

        public void Error(string message, params object[] values) => EmptyLog();

        public void Fatal(Exception exception) => EmptyLog();


        public void Fatal(string message, params object[] values) => EmptyLog();

        public void Info(string message, params object[] values) => EmptyLog();

        public void Warn(string message, params object[] values) => EmptyLog();

        public void Write(LogLevel level, string message) => EmptyLog();

        public IDisposable AddProperty(string propertyName, string propertyValue) =>
            new EmptyDisposable();

        private void EmptyLog()
        {
            // no op setter
        }
    }
}
