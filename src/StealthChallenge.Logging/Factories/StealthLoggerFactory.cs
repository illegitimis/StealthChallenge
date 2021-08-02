namespace StealthChallenge.Logging.Factories
{
    using StealthChallenge.Abstractions.Logging;
    using System;

    /// <summary>
    /// Generic logger service locator.
    /// </summary>
    public class StealthLoggerFactory : ILoggerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StealthLoggerFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public ILogger<T> Get<T>() =>
            (_serviceProvider.GetService(typeof(ILogger<T>)) as ILogger<T>)
                ?? throw new InvalidOperationException($"Cannot resolve logger for {typeof(T).FullName}");
    }
}
