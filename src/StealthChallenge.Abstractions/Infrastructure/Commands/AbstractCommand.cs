using StealthChallenge.Abstractions.Domain.Models;

namespace StealthChallenge.Abstractions.Infrastructure.Commands
{
    /// <summary>
    /// Marker interface for client commands
    /// </summary>
    /// <remarks>
    ///    GetFriendsList
    ///    AcceptInvitation
    ///    RejectInvitation
    ///    SendPick
    /// </remarks>
    public interface IClientCommand { }
}