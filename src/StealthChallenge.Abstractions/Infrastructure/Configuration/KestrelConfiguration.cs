namespace StealthChallenge.Abstractions.Infrastructure.Configuration
{
    public class KestrelConfiguration
    {
        public int TcpPort { get; set; }
        public int HttpPort { get; set; }
        public int? HttpsPort { get; set; }
    }
}
