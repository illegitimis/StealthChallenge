namespace StealthChallenge.ConsoleClient
{
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using StealthChallenge.Abstractions.Infrastructure.Commands;
    using StealthChallenge.Abstractions.Infrastructure.Configuration;
    using StealthChallenge.ConsoleClient.Models;
    using StealthChallenge.MessagePack;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public static class Program
    {
        static ClientAppSettings appSettings = new ClientAppSettings();
        static ISerializeClientCommands Serializer = new MessagePackClientCommandsSerializer();

        public static void Main(string[] args)
        { 
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                appSettings = configuration
                    .GetSection(nameof(ClientAppSettings))
                    .Get<ClientAppSettings>();

                Parser
                    .Default
                    .ParseArguments<Friends>(args)
                    .WithParsed<Friends>(async (x) => await FriendsCommandHandler(x))
                    .WithNotParsed(ConsoleWriter.WriteErrors);
            }
            catch (Exception ex)
            {
                ConsoleWriter.Error($"{ex.GetType().Name}-{ex.Message}");
            }
            Console.ReadKey();
        }

        private static async Task FriendsCommandHandler(Friends friends)
        {
            switch (friends)
            {
                case Friends _ when friends.List:
                    await GetFriendsListAsync();
                    break;

                case Friends _ when friends.Matchmake:
                    await MatchmakeAsync();
                    break;

                default:
                    break;
            }
        }

        private static async Task GetFriendsListAsync() =>
            await SendCommandAsync(
                nameof(GetFriendsListCommand),
                () => new GetFriendsListCommand { User = appSettings.User });

        private static async Task MatchmakeAsync() =>
            await SendCommandAsync(
                nameof(MatchmakeCommand),
                () => new MatchmakeCommand { User = appSettings.User });

        private static async Task SendCommandAsync(string description, Func<AbstractCommand> func)
        {
            using (var client = new TcpClient(appSettings.Ip, appSettings.Port))
            using (NetworkStream stream = client.GetStream())
            {
                Console.WriteLine(description);
                var cmd = func();
                var bytes = Serializer.Serialize(cmd);
                stream.Write(bytes);

                while (client.Connected)
                {
                    var buf = new byte[client.ReceiveBufferSize];
                    var received = 0;
                    while (received == 0)
                    {
                        received = await stream.ReadAsync(buf, 0, client.ReceiveBufferSize);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    // var span = new Span<byte>(buf, 0, received);
                    var span = new byte[received];
                    Array.Copy(buf, span, received);
                    var text = Encoding.UTF8.GetString(span);
                    ConsoleWriter.Info(text);
                }
            }
        }

    }
}
