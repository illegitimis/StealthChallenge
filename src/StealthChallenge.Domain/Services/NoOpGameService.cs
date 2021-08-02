using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Domain.Services;
using System;
using System.Threading.Tasks;

namespace StealthChallenge.Domain.Services
{
    public class NoOpGameService : IGameService
    {
        public Task<Guid> AddGameAsync(Game game) =>
            Task.FromResult(Guid.Empty);

        public Task UpdateChallengerPickAsync(RpsChoice choice) =>
            Task.CompletedTask;

        public Task UpdateInitiatorPickAsync(RpsChoice choice) =>
            Task.CompletedTask;

        public Task UpdatePicksAsync(string user, RpsChoice choice) =>
            Task.CompletedTask;
    }
}
