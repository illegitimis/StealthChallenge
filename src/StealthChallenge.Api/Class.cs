using Microsoft.Extensions.DependencyInjection;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Infrastructure.Configuration;
using StealthChallenge.Abstractions.Infrastructure.Services;
using StealthChallenge.Abstractions.Logging;
using StealthChallenge.StateMachine;
using System;

namespace StealthChallenge.Api
{
    public class RockPaperScissorsStateMachineDependenciesProvider :
        IProvideRockPaperScissorsStateMachineDependencies
    {
        private readonly IServiceProvider _sp;

        public RockPaperScissorsStateMachineDependenciesProvider(IServiceProvider sp) =>
            _sp = sp;


        public IComputeWinner GetAlgorithm() => _sp.GetRequiredService<IComputeWinner>();

        public ConfigurationSettings GetConfigurationSettings() => _sp.GetRequiredService<ConfigurationSettings>();

        public IGameService GetGameService() => _sp.GetRequiredService<IGameService>();

        public ILogger GetLogger() => _sp.GetRequiredService<ILoggerFactory>().Get<RockPaperScissorsStateMachine>();

        public ICommunicateViaTcp GetTcpCommunicator() => _sp.GetRequiredService<ICommunicateViaTcp>();

        public IUserService GetUserService() => _sp.GetRequiredService<IUserService>();
    }
}
