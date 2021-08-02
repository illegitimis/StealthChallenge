namespace StealthChallenge.Logging.Factories
{
    using StealthChallenge.Abstractions.Logging;
    using StealthChallenge.Logging.Loggers;

    public class EmptyLoggerFactory : ILoggerFactory
    {
        public ILogger<T> Get<T>()
        {
            return new EmptyLogger<T>();
        }
    }
}
