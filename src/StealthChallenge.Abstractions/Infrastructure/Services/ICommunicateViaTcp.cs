using StealthChallenge.Abstractions.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface ICommunicateViaTcp
    {
        public Task SendInviteAsync(string challenger, Guid gameId);
        Task StartNotificationAsync(string userName, Guid gameId);
        Task SendOutcomeAsync(Guid game, string user, UserOutcome userOutcome);
        /* */
        Task DisconnectAsync(string connectionId);
        Task AddConnectionAsync<TPipeWriter>(string user, string connectionId, TPipeWriter output);
        Task InviteTimeoutAsync(string initiator, Guid gameId);
    }
}
