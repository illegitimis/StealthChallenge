using System;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface IManageRunningGames
    {
        void Add(
            IRockPaperScissorsStateMachine sm,
            string initiator,
            string challenger);

        Tuple<IRockPaperScissorsStateMachine, string, string> Get(Guid gameId);

        //void Accept(string challenger, string gameId);
        //void Reject(string challenger, string gameId);
    }
}
