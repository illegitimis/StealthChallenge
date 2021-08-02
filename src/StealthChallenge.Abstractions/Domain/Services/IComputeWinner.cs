using StealthChallenge.Abstractions.Domain.Models;

namespace StealthChallenge.Abstractions.Domain.Services
{
    public interface IComputeWinner
    {
        /// <summary> Who's the winner for RPS </summary>
        /// <param name="rps1">initiator pick</param>
        /// <param name="rps2">challenger pick</param>
        /// <returns></returns>
        Outcome Compute(RpsChoice? rps1, RpsChoice? rps2);
    }
}
