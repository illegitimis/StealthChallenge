using System;
using System.Threading.Tasks;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface ICommunicateViaTcp
    {
        public Task SendInviteAsync(string challenger, Guid gameId);
        Task StartNotificationAsync(string userName, Guid gameId);
        Task SendOutcomeAsync(Guid game, string initiator, object tie);
    }
}
