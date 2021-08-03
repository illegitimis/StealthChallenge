using CommandLine;

namespace StealthChallenge.ConsoleClient.Models
{
    [Verb("friends", HelpText = "Friend related operations.")]
    public class Friends
    {
        /// <summary>
        /// friends --list
        /// </summary>
        [Option('l', "list", Default = true, HelpText = "List all friends", Required = false)]
        public bool List { get; set; }

        /// <summary>
        /// friends -m
        /// </summary>
        [Option('m', "matchmake", Default = true, HelpText = "List all possible opponents", Required = false)]
        public bool Matchmake { get; set; }
    }
}
