namespace StealthChallenge.StateMachine
{
    using System;
    using System.Threading.Tasks;
    using Stateless;
    using Stateless.Graph;
    using Timer = System.Timers.Timer;
    using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
    using ILoggerFactory = StealthChallenge.Abstractions.Logging.ILoggerFactory;
    using StealthChallenge.Abstractions.Logging;
    using static LogMessageTemplates;
    using StealthChallenge.Abstractions.Infrastructure.Services;
    using StealthChallenge.Abstractions.Domain.Services;
    using StealthChallenge.Abstractions.Infrastructure.Models;
    using StealthChallenge.Abstractions.Domain.Models;
    using State = StealthChallenge.Abstractions.Domain.Models.State;

    public class RockPaperScissorsStateMachine
    {
        private readonly ICommunicateViaTcp _tcpCommunicator;
        private readonly IGameService _gameService;
        private readonly IComputeWinner _winnerComputer;
        private readonly IUserService _userService;
        private readonly ILogger<RockPaperScissorsStateMachine> _log;
        private readonly ConfigurationSettings _configurationSettings;

        public State State { get; private set; }

        private Guid _gameId;
        private string _initiator;
        private string _challenger;
        private RpsChoice? _initiatorPick;
        private RpsChoice? _challengerPick;

        private StateMachine<State, Trigger> _machine;
        private Timer _inviteTimer;
        private Timer _picksTimer;


        public RockPaperScissorsStateMachine(
            ICommunicateViaTcp tcpCommunicator,
            IGameService gameRepo,
            ILoggerFactory logFactory,
            IComputeWinner winnerComputer,
            IUserService userService,
            ConfigurationSettings configurationSettings)
        {
            _tcpCommunicator = tcpCommunicator;
            _gameService = gameRepo;
            _log = logFactory.Get<RockPaperScissorsStateMachine>();
            _configurationSettings = configurationSettings;
            _winnerComputer = winnerComputer;
            _userService = userService;

            State = State.Zero;
            ConfigureStateMachine();
        }

        public async Task ChallengerAccept(string user)
        {
            if (!_machine.CanFire(Trigger.ReceiveChallengerConfirmation))
            {
                _log.Info(ImpossibleChallengeAccept, _gameId, State);
                return;
            }
            if (_challenger != user) throw new NotSupportedException();
            _log.Info(ReceiveChallengerConfirmation, _gameId);
            await _machine.FireAsync(Trigger.ReceiveChallengerConfirmation);
        }

        public void ChallengerReject()
        {
            if (!_machine.CanFire(Trigger.ReceiveChallengerRejection))
                return;
            _log.Info(InviteReject, _gameId);
            _machine.Fire(Trigger.ReceiveChallengerRejection);
        }

        public async Task ReceivePickAsync(string user, RpsChoice choice)
        {
            if (!_machine.CanFire(Trigger.HavePick))
            {
                _log.Info("TODO", _gameId, State);
                return;
            }

            if (user == _initiator)
            {
                _initiatorPick = choice;
                await _gameService.UpdateInitiatorPickAsync(choice);
            }
            else
            {
                _challengerPick = choice;
                await _gameService.UpdateChallengerPickAsync(choice);
            }

            await _machine.FireAsync(Trigger.HavePick);
        }

        public async Task InitiatorInviteAsync(string from, string to)
        {
            _initiator = from;
            _challenger = to;
            await _machine.FireAsync(Trigger.ReceiveInitiatorInvitation);
        }

        private void ConfigureStateMachine()
        {
            _machine = new StateMachine<State, Trigger>(() => State, s => State = s);

            _machine
                .Configure(State.Zero)
                .Permit(Trigger.ReceiveInitiatorInvitation, State.GotInitiatorInvitation);

            _machine.Configure(State.GotInitiatorInvitation)
                .OnEntryAsync(OnInitiatorInvitationAsync, State.GotInitiatorInvitation.ToString())
                .OnExitAsync(() => _tcpCommunicator.SendInviteAsync(_challenger, _gameId))
                .Permit(Trigger.SendChallengerInvitation, State.WaitingForChallengerConfirmation);

            _machine.Configure(State.WaitingForChallengerConfirmation)
                .OnEntry(() =>
                {
                    _inviteTimer = CreateTimer(
                        _configurationSettings.AcceptInvitationTimeoutInSeconds,
                        Trigger.InviteTimeOut,
                        fireAsync: false);
                })
                .OnExit(DisposeInvitationTimer)
                .Permit(Trigger.ReceiveChallengerConfirmation, State.GameStarted)
                .Permit(Trigger.ReceiveChallengerRejection, State.InvitationRejected)
                .Permit(Trigger.InviteTimeOut, State.GameInviteTimedOut);

            _machine.Configure(State.GameInviteTimedOut)
                .OnEntry(
                    () => _log.Warn(InviteTimeout, _gameId),
                    nameof(State.GameInviteTimedOut));

            _machine.Configure(State.GameStarted)
                .OnEntryAsync(OnGameStartedAsync, nameof(State.GameStarted))
                .Permit(Trigger.MakeYourPicks, State.WaitingForPicks);

            _machine.Configure(State.WaitingForPicks)
                .OnEntry(() =>
                {
                    _picksTimer = CreateTimer(
                        _configurationSettings.MakePicksTimeoutInSeconds,
                        Trigger.MakePicksTimeOut,
                        fireAsync: true);
                })
                .Permit(Trigger.HavePick, State.WaitingForSecondPick)
                .Permit(Trigger.MakePicksTimeOut, State.GamePicksTimedOut);

            _machine.Configure(State.WaitingForSecondPick)
                .Permit(Trigger.HavePick, State.GameEnded)
                .Permit(Trigger.MakePicksTimeOut, State.GamePicksTimedOut);

            _machine.Configure(State.GameEnded)
                .OnEntryAsync(EndAsync);
            
            _machine.Configure(State.GamePicksTimedOut)
                .OnEntryAsync(EndAsync);
        }

        private async Task EndAsync()
        {
            var outcome = _winnerComputer.Compute(_initiatorPick, _challengerPick);
            switch (outcome)
            {
                case Outcome.Tie:
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _initiator, UserOutcome.Tie);
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _challenger, UserOutcome.Tie);
                    await _userService.UpdateRankingAsync(_initiator, _configurationSettings.RankingTie);
                    await _userService.UpdateRankingAsync(_challenger, _configurationSettings.RankingTie);
                    break;

                case Outcome.InitiatorWin:
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _initiator, UserOutcome.Win);
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _challenger, UserOutcome.Lose);
                    await _userService.UpdateRankingAsync(_initiator, _configurationSettings.RankingWin);
                    await _userService.UpdateRankingAsync(_challenger, _configurationSettings.RankingLoss);
                    break;

                case Outcome.ChallengerWin:
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _initiator, UserOutcome.Lose);
                    await _tcpCommunicator.SendOutcomeAsync(_gameId, _challenger, UserOutcome.Win);
                    await _userService.UpdateRankingAsync(_initiator, _configurationSettings.RankingLoss);
                    await _userService.UpdateRankingAsync(_challenger, _configurationSettings.RankingWin);
                    break;
            }
            DisposePicksTimer();
        }

        private async Task OnGameStartedAsync()
        {
            _log.Info(GameStart, _gameId);
            await _tcpCommunicator.StartNotificationAsync(_initiator, _gameId);
            await _tcpCommunicator.StartNotificationAsync(_challenger, _gameId);
            _machine.Fire(Trigger.MakeYourPicks);
        }

        private async Task OnInitiatorInvitationAsync()
        {
            var date = DateTime.UtcNow;
            var entity = new Game(_initiator, _challenger, date);
            _gameId = await _gameService.AddGameAsync(entity);
            _machine.Fire(Trigger.SendChallengerInvitation);
        }

        private Timer CreateTimer(int timeoutInSeconds, Trigger onElapsedTrigger, bool fireAsync)
        {
            var timer = new Timer
            {
                /* raise only once */
                AutoReset = false,
                /* raise event */
                Enabled = true,
                /* msec */
                Interval = timeoutInSeconds * 1000,
            };
            if(fireAsync)
            {
                timer.Elapsed += async (object sender, ElapsedEventArgs e) =>
                {
                    if (!_machine.CanFire(onElapsedTrigger)) return;
                    await _machine.FireAsync(onElapsedTrigger);
                };
            }
            else
            {
                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    if (!_machine.CanFire(onElapsedTrigger)) return;
                    _machine.Fire(onElapsedTrigger);
                };
            }
            
            return timer;
        }

        private void DisposeInvitationTimer() =>
            DisposeTimer(_inviteTimer, InviteTimerDisposed);

        private void DisposePicksTimer() =>
            DisposeTimer(_picksTimer, PicksTimerDisposed);

        private void DisposeTimer(Timer timer, string messageTemplate)
        {
            timer?.Stop();
            timer?.Dispose();
            _log.Debug(messageTemplate, new object[] { _gameId });
        }
    }
}
