namespace StealthChallenge.Abstractions.Domain.Models
{
    /// <summary>
    /// verb in a past tense, just like events in a bus
    /// </summary>
    public enum State
    {
        Zero,
        GotInitiatorInvitation,
        WaitingForChallengerConfirmation,
        GameStarted,
        GameInviteTimedOut, //InvitationAcceptTimeout,
        InvitationRejected,
        WaitingForPicks, //InitiatorPicked, //FriendPicked,
        WaitingForSecondPick,
        GameEnded,
        GamePicksTimedOut, //InitiatorPickTimedOut, //FriendPickTimedOut,
        DisconnectedOnInvite, //InitiatorDisconnected, //FriendDisconnected,
        DisconnectedWhilePlaying,
    }
}
