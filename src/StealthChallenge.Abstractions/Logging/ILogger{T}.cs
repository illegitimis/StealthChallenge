namespace StealthChallenge.Abstractions.Logging
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides a contract for category based logging.
    /// </summary>
    /// <typeparam name="TCategory">logger category type</typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface ILogger<out TCategory>
        : ILogger
    {
        IDisposable AddProperty(string propertyName, string propertyValue);
    }
}
