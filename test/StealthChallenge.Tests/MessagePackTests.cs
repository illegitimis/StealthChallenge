using MessagePack;
using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.MessagePack;
using Xunit;

namespace StealthChallenge.Tests
{
    public class MessagePackTests
    {
        [Fact]
        public void One()
        {
            var sut = new MessagePackClientCommandsSerializer();
            var cmd = new GetFriendsListCommand { User = "user" };
            byte[] bytes = sut.Serialize(cmd);
            var abstractCmd = sut.Deserialize(bytes);
            var cmd2 = Assert.IsType<GetFriendsListCommand>(abstractCmd);
            Assert.Equal(cmd.User, cmd2.User);
        }

        [Fact]
        public void PolymorphicArray()
        {
            const string user = nameof(user), game = nameof(game);

            var cmds = new AbstractCommand[]
            {
                new GetFriendsListCommand { User = user },
                new AcceptInvitationCommand { User = user, GameId = game },
                new RejectInvitationCommand { User = user, GameId = game },
                new SendPickCommand { User = user, GameId = game, Pick = RpsChoice.Paper }
            };
            byte[] bytes = MessagePackSerializer.Serialize(cmds);
            var cmds2 = MessagePackSerializer.Deserialize<AbstractCommand[]>(bytes);
            Assert.Equal(4, cmds2.Length);
        }
    }   
}
