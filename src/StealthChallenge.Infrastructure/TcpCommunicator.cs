using System;
using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Infrastructure
{
    public class TcpCommunicator : ICommunicateViaTcp
    {
        private readonly ConcurrentDictionary<string, string> _userConnections;
        private readonly ConcurrentDictionary<string, PipeWriter> _pipes;

        public TcpCommunicator()
        {
            _userConnections = new ConcurrentDictionary<string, string>();
            _pipes = new ConcurrentDictionary<string, PipeWriter>();
        }

        public Task AddConnectionAsync<TPipeWriter>(
            string user,
            string connectionId,
            TPipeWriter output)
        {
            _userConnections.GetOrAdd(user, connectionId);
            _pipes.GetOrAdd(connectionId, output as PipeWriter);
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync(string connectionId)
        {
            if (!_pipes.ContainsKey(connectionId)) return;
            var pipe = _pipes[connectionId];
            await pipe.CompleteAsync();
            var kvp = _userConnections.FirstOrDefault(x => x.Value == connectionId);
            _userConnections.TryRemove(kvp);
        }

        public async Task InviteTimeoutAsync(string initiator, Guid gameId)
        {
            var connection = _userConnections[initiator];
            var pipe = _pipes[connection];
            var s = $"Invite for game {gameId} timed out";
            await pipe.WriteAsync(Encoding.UTF8.GetBytes(s));
        }

        public async Task SendInviteAsync(string challenger, Guid gameId)
        {
            var connection = _userConnections[challenger];
            var pipe = _pipes[connection];
            var s = $"You have been invited to game {gameId}";
            await pipe.WriteAsync(Encoding.UTF8.GetBytes(s));
        }

        public async Task SendOutcomeAsync(Guid game, string user, UserOutcome userOutcome)
        {
            var connection = _userConnections[user];
            var pipe = _pipes[connection];
            var s = $"Game {game} outcome {userOutcome}";
            await pipe.WriteAsync(Encoding.UTF8.GetBytes(s));
        }

        public async Task StartNotificationAsync(string userName, Guid gameId)
        {
            var connection = _userConnections[userName];
            var pipe = _pipes[connection];
            var s = $"Game {gameId} started";
            await pipe.WriteAsync(Encoding.UTF8.GetBytes(s));
        }
    }
}
