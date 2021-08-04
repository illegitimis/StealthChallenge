using System;
using System.Threading.Tasks;
using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Infrastructure
{
    public class NoOpCommunicateViaTcp : ICommunicateViaTcp
    {
        public Task AddConnectionAsync<TPipeWriter>(string user, string connectionId, TPipeWriter output) => Task.CompletedTask;

        public Task DisconnectAsync(string connectionId) => Task.CompletedTask;

        public Task InviteTimeoutAsync(string initiator, Guid gameId) => Task.CompletedTask;

        public Task SendInviteAsync(string challenger, Guid gameId) => Task.CompletedTask;

        public Task SendOutcomeAsync(Guid game, string user, UserOutcome userOutcome) => Task.CompletedTask;

        public Task StartNotificationAsync(string userName, Guid gameId) => Task.CompletedTask;
    }
}
