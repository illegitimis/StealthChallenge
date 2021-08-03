using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Logging;
using StealthChallenge.Abstractions.Infrastructure.Configuration;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface IProvideRockPaperScissorsStateMachineDependencies
    {
        ICommunicateViaTcp GetTcpCommunicator();
        IGameService GetGameService();
        IComputeWinner GetAlgorithm();
        IUserService GetUserService();
        ILogger GetLogger();
        ConfigurationSettings GetConfigurationSettings();
    }
}
