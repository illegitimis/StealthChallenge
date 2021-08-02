namespace StealthChallenge.Logging
{
    using Serilog.Core;
    using Serilog.Events;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// https://github.com/nblumhardt/serilog-timings/blob/master/test/SerilogTimings.Tests/Support/CollectingLogger.cs
    /// </summary>
    public class CollectionSink : ILogEventSink
    {
        public ConcurrentQueue<LogEvent> Events { get; } = new ConcurrentQueue<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            Events.Enqueue(logEvent);
#if DEBUG
            Console.WriteLine($"Observed event {logEvent}");
#endif
        }

        public void Purge()
        {
            while (!Events.IsEmpty)
                Events.TryDequeue(out LogEvent _);
        }
    }
}