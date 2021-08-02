using NSubstitute;
using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Infrastructure.Models;
using StealthChallenge.Abstractions.Infrastructure.Services;
using StealthChallenge.Abstractions.Logging;
using StealthChallenge.StateMachine;
using System;
using System.Threading.Tasks;
using Xunit;
using Game = StealthChallenge.Abstractions.Domain.Models.Game;
using static StealthChallenge.StateMachine.LogMessageTemplates;

namespace StealthChallenge.Tests
{
    public class StateMachineTests
    {
        private readonly ILogger<RockPaperScissorsStateMachine> _log;
        private readonly ICommunicateViaTcp _tcpCommunicator;
        private readonly IGameService _gameRepo;
        private readonly ILoggerFactory _logFactory;
        private readonly IComputeWinner _algo;
        private readonly IUserService _userService;

        public StateMachineTests()
        {
            _tcpCommunicator = Substitute.For<ICommunicateViaTcp>();
            _gameRepo = Substitute.For<IGameService>();
            _log = Substitute.For<ILogger<RockPaperScissorsStateMachine>>();
            _logFactory = Substitute.For<ILoggerFactory>();
            _logFactory
                .Get<RockPaperScissorsStateMachine>()
                .Returns(_log);
            _algo = Substitute.For<IComputeWinner>();
            _userService = Substitute.For<IUserService>();
        }

        [Fact]
        public async Task InvitationSent_Then_GameEntityCreated_And_ChallengerInvited()
        {
            var sm = Sut(ConfigurationSettings01);

            await sm.InitiatorInviteAsync(from, to);

            Assert.Equal(State.WaitingForChallengerConfirmation, sm.State);
            await _gameRepo
                .Received(1)
                .AddGameAsync(Arg.Any<Game>());
            await _tcpCommunicator
                .Received(1)
                .SendInviteAsync(Arg.Any<string>(), Arg.Any<Guid>());
        }

        [Fact]
        public async Task NoChallengerResponse_Then_InviteTimeout()
        {
            var sm = Sut(ConfigurationSettings01);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);

            await sm.InitiatorInviteAsync(from, to);
            await Task.Delay(TimeSpan.FromSeconds(2*Seconds01));

            Assert.Equal(State.GameInviteTimedOut, sm.State);
            _log
                .Received(1)
                .Warn(InviteTimeout, guid);
        }

        [Fact]
        public async Task ChallengerAcceptsInvitationBeforeTimeout_Then_GameStarted_And_InviteTimerDisposed()
        {
            var sm = Sut(ConfigurationSettings03);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);

            await sm.InitiatorInviteAsync(from, to);
            await Task
                .Delay(TimeSpan.FromSeconds(Seconds01))
                .ContinueWith(t => sm.ChallengerAccept(to));

            Assert.Equal(State.WaitingForPicks, sm.State);
            await _tcpCommunicator
                .Received(1)
                .SendInviteAsync(Arg.Any<string>(), guid);
            _log
                .Received(1)
                .Info(ReceiveChallengerConfirmation, guid);
            CheckTimerDispose(invite: true);
        }

        [Fact]
        public async Task ChallengerAcceptsInvitationAfterTimeout_Then_InviteTimeOut_And_InviteTimerDisposed()
        {
            var sm = Sut(ConfigurationSettings01);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);

            await sm.InitiatorInviteAsync(from, to);
            await Task
                .Delay(TimeSpan.FromSeconds(Seconds03))
               .ContinueWith(t => sm.ChallengerAccept(to));

            Assert.Equal(State.GameInviteTimedOut, sm.State);
            await _tcpCommunicator
                .Received(1)
                .SendInviteAsync(Arg.Any<string>(), guid);
            _log
                .Received(1)
                .Info(ImpossibleChallengeAccept, guid, State.GameInviteTimedOut);
            CheckTimerDispose();
        }

        [Fact]
        public async Task ChallengerRejectsInvitation_Then_InvitationRejected_And_InviteTimerDisposed()
        {
            var sm = Sut(ConfigurationSettings01);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);

            await sm.InitiatorInviteAsync(from, to);
            sm.ChallengerReject();

            Assert.Equal(State.InvitationRejected, sm.State);
            _log
                .Received(1)
                .Info(InviteReject, guid);
            CheckTimerDispose(invite: true);
        }

        [Fact]
        public async Task BothTimeoutOnPicks_Then_Tie_And_Dispose()
        {
            var sm = Sut(ConfigurationSettings03);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);

            await sm.InitiatorInviteAsync(from, to);
            await Task.Delay(OneSecond);
            await sm.ChallengerAccept(to);
            await Task.Delay(TimeSpan.FromSeconds(Seconds01+Seconds03));

            Assert.Equal(State.GamePicksTimedOut, sm.State);
            _algo
                .Received(1)
                .Compute(null, null);
            await _userService
                .Received(1)
                .UpdateRankingAsync(from, 1);
            await _userService
                .Received(1)
                .UpdateRankingAsync(to, 1);
            CheckTimerDispose(false);
        }

        [Fact]
        public async Task ChallengerTimesOutOnPicks_Then_InitiatorWin()
        {
            var sm = Sut(ConfigurationSettings03);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);
            _algo
                .Compute(RpsChoice.Scissors, null)
                .Returns(Outcome.InitiatorWin);

            await sm.InitiatorInviteAsync(from, to);
            await Task.Delay(OneSecond);
            await sm.ChallengerAccept(to);
            await Task.Delay(OneSecond);
            await sm.ReceivePickAsync("from", RpsChoice.Scissors);
            await Task.Delay(TimeSpan.FromSeconds(Seconds03));

            Assert.Equal(State.GamePicksTimedOut, sm.State);
            await _tcpCommunicator
                .Received(1)
                .SendOutcomeAsync(guid, from, UserOutcome.Win);
            await _tcpCommunicator
                .Received(1)
                .SendOutcomeAsync(guid, to, UserOutcome.Lose);
            await _userService
                .Received(1)
                .UpdateRankingAsync(from, 3);
            await _userService
                .Received(1)
                .UpdateRankingAsync(to, -3);
            CheckTimerDispose(false);
        }

        [Fact]
        public async Task PositiveCase()
        {
            var sm = Sut(ConfigurationSettingsProduction);
            var guid = Guid.NewGuid();
            _gameRepo
                .AddGameAsync(Arg.Any<Game>())
                .Returns(guid);
            var ts1 = Task.Delay(TimeSpan.FromSeconds(Seconds01));

            await sm.InitiatorInviteAsync(from, to);
            await ts1;
            await sm.ChallengerAccept(to);
            await ts1;
            await sm.ReceivePickAsync("from", RpsChoice.Paper);
            await ts1;
            await sm.ReceivePickAsync("to", RpsChoice.Rock);
            await ts1;

            Assert.Equal(State.GameEnded, sm.State);
            await _gameRepo
                .Received(1)
                .UpdateInitiatorPickAsync(RpsChoice.Paper);
            await _gameRepo
                .Received(1)
                .UpdateChallengerPickAsync(RpsChoice.Rock);
            _algo
                .Received(1)
                .Compute(RpsChoice.Paper, RpsChoice.Rock);
            _log
                .Received(1)
                .Info(GameStart, guid);
            CheckTimerDispose(false);
        }

        [Fact]
        public async Task DotGraph()
        {
            var sm = Sut(ConfigurationSettingsProduction);
            var dotGraph = sm.ToString();
            Assert.NotNull(dotGraph);
        }

        private void CheckTimerDispose(bool invite = true) => _log
                .Received(1)
                .Debug(
                    invite ? InviteTimerDisposed : PicksTimerDisposed,
                    Arg.Any<object[]>(),
                    "DisposeTimer");

        private RockPaperScissorsStateMachine Sut(ConfigurationSettings cs) =>
            new RockPaperScissorsStateMachine(
                _tcpCommunicator,
                _gameRepo,
                _logFactory,
                _algo,
                _userService,
                cs);

        private static readonly ConfigurationSettings ConfigurationSettings01 =
            new ConfigurationSettings { AcceptInvitationTimeoutInSeconds = Seconds01 };

        private static readonly ConfigurationSettings ConfigurationSettings03 =
            new ConfigurationSettings
            { 
                AcceptInvitationTimeoutInSeconds = Seconds03,
                MakePicksTimeoutInSeconds = Seconds03
            };

        private static readonly ConfigurationSettings ConfigurationSettingsProduction =
            new ConfigurationSettings
            {
                AcceptInvitationTimeoutInSeconds = 3 * 60,
                MakePicksTimeoutInSeconds = 10 * 60
            };

        const int Seconds03 = 3;
        const int Seconds01 = 1;
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(Seconds01);

        const string to = nameof(to);
        const string from = nameof(from);
    }
}
