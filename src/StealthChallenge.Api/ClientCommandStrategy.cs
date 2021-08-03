using StealthChallenge.Abstractions.Infrastructure.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using StealthChallenge.MessagePack;
using System.Text;
using StealthChallenge.Abstractions.Domain.Services;
using System.IO.Pipelines;
using StealthChallenge.Abstractions.Infrastructure.Services;
using StealthChallenge.Abstractions.Logging;
using StealthChallenge.StateMachine;
using System.Threading.Tasks;
using System.IO.Pipelines;

namespace StealthChallenge.Api
{
    public interface IClientCommandStrategy
    {
        public Task HandleAsync(IClientCommand clientCommand, PipeWriter pipeWriter);
    }

    public interface IManageRunningGames
    {
        void AddStateMachine(
            IRockPaperScissorsStateMachine sm,
            string initiator,
            string challenger);
        Task DisconnectAsync(string connectionId);
        Task AddConnectionAsync(string user, string connectionId, PipeWriter output);
    }

    public class ClientCommandStrategy : IClientCommandStrategy
    {
        private readonly IUserService _userService;
        private readonly IMakeMatches _matchmaker;
        private readonly ILogger<ClientCommandStrategy> _log;
        private readonly IProvideRockPaperScissorsStateMachineDependencies _smDepends;
        private readonly IManageRunningGames _gm;

        public ClientCommandStrategy(
            IUserService userService,
            IMakeMatches matchmaker,
            ILoggerFactory factory,
            IProvideRockPaperScissorsStateMachineDependencies smDepends,
            IManageRunningGames gm)
        {
            _userService = userService;
            _matchmaker = matchmaker;
            _log = factory.Get<ClientCommandStrategy>();
            _smDepends = smDepends;
            _gm = gm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientCommand"></param>
        /// <param name="pipeWriter">connection.Transport.Output</param>
        /// <returns></returns>
        public async Task HandleAsync(IClientCommand clientCommand, PipeWriter pipeWriter)
        {
            _log.Info("Handling command {@command}", clientCommand);
            switch (clientCommand)
            {
                case GetFriendsListCommand getFriendsList:
                    var userRankProjections = await _userService
                        .FindFriendsAsync(getFriendsList.User)
                        .ConfigureAwait(false);
                    var s = userRankProjections
                        .Select(x => $"{x.Name} {x.Ranking}")
                        .Aggregate(String.Empty, (y, z) => $"{y}{Environment.NewLine}{z}");
                    var bytes = Encoding.UTF8.GetBytes(s);
                    await pipeWriter
                        .WriteAsync(bytes)
                        .ConfigureAwait(false);
                    break;

                case MatchmakeCommand matchmakeCommand:
                    var strings = await _matchmaker
                        .Match(matchmakeCommand.User)
                        .ConfigureAwait(false);
                    s = string.Join(Environment.NewLine, strings);
                    bytes = Encoding.UTF8.GetBytes(s);
                    await pipeWriter
                        .WriteAsync(bytes)
                        .ConfigureAwait(false);
                    break;

                case InviteCommand inviteCommand:
                    var initiator = inviteCommand.User;
                    var challenger = inviteCommand.Challenger;
                    var sm = new RockPaperScissorsStateMachine(_smDepends);
                    await sm
                        .InitiatorInviteAsync(initiator, challenger)
                        .ConfigureAwait(false);
                    _gm.AddStateMachine(sm, initiator, challenger);
                    break;

                default:
                    break;
            }
        }
    }
}
