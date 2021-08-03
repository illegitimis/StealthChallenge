using MessagePack;
using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Infrastructure.Commands;

namespace StealthChallenge.MessagePack
{
    [Union(0, typeof(GetFriendsListCommand))]
    [Union(1, typeof(AcceptInvitationCommand))]
    [Union(2, typeof(RejectInvitationCommand))]
    [Union(3, typeof(SendPickCommand))]
    [Union(4, typeof(MatchmakeCommand))]
    [MessagePackObject]
    public abstract class AbstractCommand : IClientCommand
    {
        [Key(0)]
        public string User { get; set; }
    }

    [MessagePackObject]
    public class GetFriendsListCommand : AbstractCommand
    {
    }

    [MessagePackObject]
    public class MatchmakeCommand : AbstractCommand
    {
    }

    public abstract class AbstractGameCommand : AbstractCommand
    {
        [Key(1)]
        public string GameId { get; set; }
    }

    [MessagePackObject]
    public class AcceptInvitationCommand : AbstractGameCommand
    {
    }

    [MessagePackObject]
    public class RejectInvitationCommand : AbstractGameCommand
    {
    }

    [MessagePackObject]
    public class SendPickCommand : AbstractGameCommand
    {
        [Key(2)]
        public RpsChoice Pick { get; set; }
    }
}
