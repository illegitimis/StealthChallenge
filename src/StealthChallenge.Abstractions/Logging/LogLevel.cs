namespace StealthChallenge.Abstractions.Logging
{
    /// <summary>
    /// Identifies the type of event that has caused the trace.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Fatal error or application crash.
        /// </summary>
        Fatal = 1,

        /// <summary>
        /// Recoverable error.
        /// </summary>
        Error = 2,

        /// <summary>
        ///  Noncritical problem.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Informational message.
        /// </summary>
        Information = 8,
       
        /// <summary>
        /// Debugging trace
        /// </summary>
        Debug = 16,
    }
}