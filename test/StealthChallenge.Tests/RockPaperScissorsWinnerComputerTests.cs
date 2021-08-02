using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Domain.Services;
using Xunit;

namespace StealthChallenge.Tests
{
    public class RockPaperScissorsWinnerComputerTests
    {
        private readonly IComputeWinner _algo = new RockPaperScissorsWinnerComputer();

        [Theory]
        [InlineData(null,null, Outcome.Tie)]
        [InlineData(RpsChoice.Rock, RpsChoice.Rock, Outcome.Tie)]
        [InlineData(RpsChoice.Paper, RpsChoice.Paper, Outcome.Tie)]
        [InlineData(RpsChoice.Scissors, RpsChoice.Scissors, Outcome.Tie)]
        [InlineData(null, RpsChoice.Rock, Outcome.ChallengerWin)]
        [InlineData(null, RpsChoice.Paper, Outcome.ChallengerWin)]
        [InlineData(null, RpsChoice.Scissors, Outcome.ChallengerWin)]
        [InlineData(RpsChoice.Rock, RpsChoice.Paper , Outcome.ChallengerWin)]
        [InlineData(RpsChoice.Paper, RpsChoice.Scissors , Outcome.ChallengerWin)]
        [InlineData(RpsChoice.Scissors, RpsChoice.Rock , Outcome.ChallengerWin)]
        [InlineData(RpsChoice.Rock, null, Outcome.InitiatorWin)]
        [InlineData(RpsChoice.Paper, null, Outcome.InitiatorWin)]
        [InlineData(RpsChoice.Scissors, null, Outcome.InitiatorWin)]
        [InlineData(RpsChoice.Paper, RpsChoice.Rock, Outcome.InitiatorWin)]
        [InlineData(RpsChoice.Scissors, RpsChoice.Paper, Outcome.InitiatorWin)]
        [InlineData(RpsChoice.Rock, RpsChoice.Scissors, Outcome.InitiatorWin)]
        public void Should(RpsChoice? rps1, RpsChoice? rps2, Outcome expected)
        {
            var result = _algo.Compute(rps1, rps2);
            Assert.Equal(expected, result);
        }
    }
}
