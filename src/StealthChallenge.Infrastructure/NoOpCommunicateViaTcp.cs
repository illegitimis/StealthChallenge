using System;
using System.Threading.Tasks;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Infrastructure
{
    public class NoOpCommunicateViaTcp : ICommunicateViaTcp
    {
        public Task SendInviteAsync(string challenger, Guid gameId) => Task.CompletedTask;

        public Task SendOutcomeAsync(Guid game, string initiator, object tie) => Task.CompletedTask;

        public Task StartNotificationAsync(string userName, Guid gameId) => Task.CompletedTask;
    }
}
