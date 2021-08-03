using CommandLine;

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

    [Verb("invite", HelpText = "Invitations.")]
    public class Invitations
    {
        /// <summary>
        /// invite --send user
        /// </summary>
        [Option('s', "send", Default = false, HelpText = "Send game invite", Required = false)]
        public string User { get; set; }

        /// <summary>
        /// invite -a gameid
        /// </summary>
        [Option('a', "accept", Default = false, HelpText = "accept game invite", Required = false)]
        public string AcceptGame { get; set; }

        /// <summary>
        /// invite --reject gameid
        /// </summary>
        [Option('r', "reject", Default = false, HelpText = "reject game invite", Required = false)]
        public string RejectGame { get; set; }
    }
}
