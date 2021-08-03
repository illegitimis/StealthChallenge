﻿using StealthChallenge.Abstractions.Infrastructure.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using StealthChallenge.MessagePack;
using System.Text;
using StealthChallenge.Abstractions.Domain.Services;
using System.IO.Pipelines;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Api
{
    public interface IClientCommandStrategy
    {
        public Task HandleAsync(IClientCommand clientCommand, PipeWriter pipeWriter);
    }

    public class ClientCommandStrategy : IClientCommandStrategy
    {
        private readonly IUserService _userService;
        private readonly IMakeMatches _matchmaker;

        public ClientCommandStrategy(IUserService userService, IMakeMatches matchmaker)
        {
            _userService = userService;
            _matchmaker = matchmaker;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientCommand"></param>
        /// <param name="pipeWriter">connection.Transport.Output</param>
        /// <returns></returns>
        public async Task HandleAsync(IClientCommand clientCommand, PipeWriter pipeWriter)
        {
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

                default:
                    break;
            }
        }
    }
}