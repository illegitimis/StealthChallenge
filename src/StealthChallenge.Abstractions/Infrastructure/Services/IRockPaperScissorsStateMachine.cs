using StealthChallenge.Abstractions.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface IRockPaperScissorsStateMachine
    {
        Guid GameId { get; }
        Task ChallengerAccept(string user);
        void ChallengerReject(string user);
        Task ReceivePickAsync(string user, RpsChoice choice);
    }
}