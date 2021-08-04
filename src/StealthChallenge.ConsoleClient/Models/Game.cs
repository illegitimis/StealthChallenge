using CommandLine;
using StealthChallenge.Abstractions.Domain.Models;

namespace StealthChallenge.ConsoleClient.Models
{
    [Verb("game", HelpText = "Friend related operations.")]
    public class Game
    {
        /// <summary>
        /// game ?
        /// </summary>
        [Option('g', "id", Default = "", HelpText = "Game identifier", Required = true)]
        public string Id { get; set; }

        /// <summary>
        /// friends -m
        /// </summary>
        [Option('p', "pick", HelpText = "rock, paper, scissors", Required = true)]
        public RpsChoice Pick { get; set; }
    }
}
