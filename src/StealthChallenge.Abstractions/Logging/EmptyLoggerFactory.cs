namespace StealthChallenge.Abstractions.Logging
{
    public class EmptyLoggerFactory : ILoggerFactory
    {
        public ILogger<T> Get<T>()
        {
            return new EmptyLogger<T>();
        }
    }
}
