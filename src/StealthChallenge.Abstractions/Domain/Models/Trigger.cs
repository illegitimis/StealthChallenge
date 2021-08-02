namespace StealthChallenge.Abstractions.Domain.Models
{
    /// <summary>
    /// imperative action present
    /// </summary>
    public enum Trigger
    {
        ReceiveInitiatorInvitation,
        SendChallengerInvitation,
        ReceiveChallengerConfirmation,
        InviteTimeOut,
        ReceiveChallengerRejection,
        MakeYourPicks,
        MakePicksTimeOut,
        HavePick
        //InitiateInvite,
        //RespondToInvite,
        //DisconnectInitiator,
        //DisconnectFriend,
        //ReceiveInitiatorPick,
        //ReceiveFriendPick
    }
}
