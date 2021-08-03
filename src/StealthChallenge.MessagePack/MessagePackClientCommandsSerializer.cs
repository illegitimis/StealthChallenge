using System;
using MessagePack;
using StealthChallenge.Abstractions.Infrastructure.Commands;

namespace StealthChallenge.MessagePack
{
    public class MessagePackClientCommandsSerializer : ISerializeClientCommands
    {
        public IClientCommand Deserialize(byte[] bytes)
        {
            try
            {
                return MessagePackSerializer.Deserialize<AbstractCommand>(bytes);
            }
            catch
            {
                return MessagePackSerializer.Deserialize<IClientCommand>(bytes);
            }
        }

        public byte[] Serialize(IClientCommand clientCommand)
        {
            return (clientCommand is AbstractCommand cmd)
                ? MessagePackSerializer.Serialize(cmd)
                : MessagePackSerializer.Serialize(clientCommand, options);
        }

        private static readonly MessagePackSerializerOptions options =
          global::MessagePack.Resolvers.ContractlessStandardResolver.Options;
    }
}
