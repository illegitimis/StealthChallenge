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

namespace StealthChallenge.Api
{

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
                    _gm.Add(sm, initiator, challenger);
                    await sm
                        .InitiatorInviteAsync(initiator, challenger)
                        .ConfigureAwait(false);
                    break;

                case AcceptInvitationCommand acceptCmd:
                    if (!Guid.TryParse(acceptCmd.GameId, out Guid guid)) return;
                    var tuple = _gm.Get(guid);
                    if (tuple.Item3 == acceptCmd.User)
                        await tuple.Item1.ChallengerAccept(acceptCmd.User);
                    break;

                case RejectInvitationCommand rejectCmd:
                    if (!Guid.TryParse(rejectCmd.GameId, out guid)) return;
                    tuple = _gm.Get(guid);
                    if (tuple.Item3 == rejectCmd.User)
                        tuple.Item1.ChallengerReject(rejectCmd.User);
                    break;

                case SendPickCommand cmd:
                    if (!Guid.TryParse(cmd.GameId, out guid)) return;
                    tuple = _gm.Get(guid);
                    await tuple.Item1.ReceivePickAsync(cmd.User, cmd.Pick);
                    break;

                default:
                    break;
            }
        }
    }
}
