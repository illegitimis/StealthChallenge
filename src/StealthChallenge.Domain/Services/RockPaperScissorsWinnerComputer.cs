using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Domain.Services;

namespace StealthChallenge.Domain.Services
{
    public class RockPaperScissorsWinnerComputer : IComputeWinner
    {
        public Outcome Compute(RpsChoice? rps1, RpsChoice? rps2)
        {
            switch ((rps1, rps2))
            {
                default:
                case (null, null):
                case (RpsChoice.Rock, RpsChoice.Rock):
                case (RpsChoice.Paper, RpsChoice.Paper):
                case (RpsChoice.Scissors, RpsChoice.Scissors):
                    return Outcome.Tie;

                case (null, _):
                /* paper covers rock */
                case (RpsChoice.Rock, RpsChoice.Paper):
                /* scissors cuts paper */
                case (RpsChoice.Paper, RpsChoice.Scissors):
                /* rock damages scissors */
                case (RpsChoice.Scissors, RpsChoice.Rock):
                    return Outcome.ChallengerWin;

                case (_, null):
                /* paper covers rock */
                case (RpsChoice.Paper, RpsChoice.Rock):
                /* scissors cuts paper */
                case (RpsChoice.Scissors, RpsChoice.Paper):
                /* rock damages scissors */
                case (RpsChoice.Rock, RpsChoice.Scissors):
                    return Outcome.InitiatorWin;
            }
        }
    }
}
