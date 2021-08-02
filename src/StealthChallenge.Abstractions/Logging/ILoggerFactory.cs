namespace StealthChallenge.Abstractions.Logging
{
    public interface ILoggerFactory
    {
        ILogger<T> Get<T>();
    }
}
