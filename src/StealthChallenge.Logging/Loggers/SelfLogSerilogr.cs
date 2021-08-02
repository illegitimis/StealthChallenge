namespace StealthChallenge.Logging.Loggers
{
    using Serilog.Debugging;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    public class SelfLogSerilogr : Serilogr, IDisposable
    {
        /// <summary>
        /// managed object should be disposed
        /// </summary>
        private readonly TextWriter _textWriter;

        public SelfLogSerilogr()
        {
            // subscribe to its internal events and write them to your debug window or a console.
            if (IsDebug)
            {
                SelfLog.Enable(Console.Error);
                var date = DateTime.UtcNow.ToString(DateFormat);
                _textWriter = new StreamWriter($"{date}.log");
                SelfLog.Enable(_textWriter);
            }
        }

        /// <summary>
        /// detect redundant calls
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _textWriter?.Dispose();
                _disposedValue = true;
            }
        }
    }
}
