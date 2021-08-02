using StealthChallenge.Abstractions.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StealthChallenge.Abstractions.Domain.Services
{
    public interface IGameService
    {
        Task<Guid> AddGameAsync(Game game);
        Task UpdateInitiatorPickAsync(RpsChoice choice);
        Task UpdateChallengerPickAsync(RpsChoice choice);
    }
}
