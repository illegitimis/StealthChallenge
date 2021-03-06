using CommandLine;
using System;

namespace StealthChallenge.ConsoleClient.Models
{
    [Verb("friends", HelpText = "Friend related operations.")]
    public class Friends
    {
        /// <summary>
        /// friends --list
        /// </summary>
        [Option('l', "list", Default = false, HelpText = "List all friends", Required = false)]
        public bool List { get; set; }

        /// <summary>
        /// friends -m
        /// </summary>
        [Option('m', "matchmake", Default = false, HelpText = "List all possible opponents", Required = false)]
        public bool Matchmake { get; set; }
    }
}
