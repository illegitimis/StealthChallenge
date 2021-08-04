using System;
using System.Collections.Concurrent;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Infrastructure
{
    public class GameManager : IManageRunningGames
    {
        private readonly ConcurrentDictionary<Guid,
            Tuple<IRockPaperScissorsStateMachine,string,string>> _states;

        public GameManager()
        {
            _states = new ConcurrentDictionary<Guid,
                Tuple<IRockPaperScissorsStateMachine, string, string>>();
        }

        public Tuple<IRockPaperScissorsStateMachine, string, string> Get(Guid gameId) =>
            _states[gameId];

        public void Add(IRockPaperScissorsStateMachine sm, string initiator, string challenger)
        {
            var tuple = new Tuple<IRockPaperScissorsStateMachine, string, string>(
               sm, initiator, challenger);
            _states.TryAdd(sm.GameId, tuple);
        }
    }
}
