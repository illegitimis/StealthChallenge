namespace StealthChallenge.Abstractions.Domain.Models
{
    using System;

    public class Game
    {
        public Game(
            string from,
            string to,
            DateTime invitationDate)
        {
            From = from;
            To = to;
            InvitationDate = invitationDate;
            // StartDate = startDate;
        }
        public /*Guid*/int Id { get; set; }

        public string From { get; }
        public RpsChoice? InitiatorPick { get; }


        public string To { get; }
        public RpsChoice? ChallengerPick { get; }


        public DateTime InvitationDate { get; }

        public DateTime StartDate { get; set; }

        //public StateType State { get; set; }

        /* GameInviteTimedOut, InvitationRejected, GameEnded, GamePicksTimedOut, Disconnected */
        public State State { get; set; }
    }
}
