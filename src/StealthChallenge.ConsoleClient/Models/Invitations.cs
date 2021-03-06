using CommandLine;

namespace StealthChallenge.ConsoleClient.Models
{
    [Verb("invite", HelpText = "Invitations.")]
    public class Invitations
    {
        /// <summary>
        /// invite --send user
        /// </summary>
        [Option('s', "send", Default = "", HelpText = "Send game invite", Required = false)]
        public string User { get; set; }

        /// <summary>
        /// invite -a gameid
        /// </summary>
        [Option('a', "accept", Default = "", HelpText = "accept game invite", Required = false)]
        public string AcceptGame { get; set; }

        /// <summary>
        /// invite --reject gameid
        /// </summary>
        [Option('r', "reject", Default = "", HelpText = "reject game invite", Required = false)]
        public string RejectGame { get; set; }
    }
}
