namespace StealthChallenge.Abstractions.Infrastructure.Commands
{
    public interface ISerializeClientCommands
    {
        byte[] Serialize(IClientCommand clientCommand);
        IClientCommand Deserialize(byte[] bytes);
    }
}