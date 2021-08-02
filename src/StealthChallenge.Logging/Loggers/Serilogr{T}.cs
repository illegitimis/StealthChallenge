namespace StealthChallenge.Logging.Loggers
{
    using StealthChallenge.Abstractions.Logging;
    using Serilog.Context;
    using System;
    using static PropertyNames;
    using ISerilogLogger = Serilog.ILogger;

    public class Serilogr<T> : Serilogr, ILogger<T>
    {
        public Serilogr(ISerilogLogger log = null)
            : base(log)
        {
            Log = Log
                .ForContext<T>()
                .ForContext(ClassName, typeof(T).Name);
        }

        public IDisposable AddProperty(string propertyName, string propertyValue)
        {
            var disposable = LogContext.PushProperty(propertyName, propertyValue);
            return disposable;
        }
    }
}
