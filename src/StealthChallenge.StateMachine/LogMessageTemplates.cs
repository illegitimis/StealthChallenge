namespace StealthChallenge.StateMachine
{
    public static class LogMessageTemplates
    {
        internal const string InviteTimerDisposed =
            "Invite timer disposed for game " + GameIdNamedParameter;

        internal const string PicksTimerDisposed =
            "Picks timer disposed for game " + GameIdNamedParameter;

        internal const string GameStart =
          "Game " + GameIdNamedParameter + " started";

        internal const string InviteTimeout =
            "Game " + GameIdNamedParameter + " invite timed out";

        internal const string InviteReject =
            "Challenger rejected game " + GameIdNamedParameter;

        internal const string ReceiveChallengerConfirmation =
            "Received challenger confirmation for game " + GameIdNamedParameter;

        internal const string ImpossibleChallengeAccept =
          "Impossible to accept challenge invitation for game " +
            GameIdNamedParameter +
            ". Current state" +
            StateNamedParameter;

        private const string GameIdNamedParameter = "{gameId}";
        private const string StateNamedParameter = "{state}";
    }
}
