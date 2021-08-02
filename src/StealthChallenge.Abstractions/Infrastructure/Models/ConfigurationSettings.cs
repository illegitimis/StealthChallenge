namespace StealthChallenge.Abstractions.Infrastructure.Models
{
    public class ConfigurationSettings
    {
        public int AcceptInvitationTimeoutInSeconds { get; set; } = 180;
        public int MakePicksTimeoutInSeconds { get; set; } = 600;
        public int RankingTie { get; set; } = 1;
        public int RankingWin { get; set; } = 3;
        public int RankingLoss { get; set; } = -3;
    }
}
