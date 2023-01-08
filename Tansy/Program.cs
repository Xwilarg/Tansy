using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text.Json;

namespace Tansy
{
    public sealed class Program
    {
        public static async Task Main()
        {
            await new Program().StartAsync();
        }

        public async Task StartAsync()
        {
            await Log.LogAsync(new LogMessage(LogSeverity.Info, "Setup", "Initialising bot"));

            // Setting Logs callback
            StaticObjects.Client.Log += Log.LogAsync;

            // Load credentials
            if (!File.Exists("Keys/Credentials.json"))
                throw new FileNotFoundException("Missing Credentials file");
            var credentials = JsonSerializer.Deserialize<Credentials>(File.ReadAllText("Keys/Credentials.json"));
            if (credentials == null)
                throw new NullReferenceException("No token found");

            // Discord callbacks
            StaticObjects.Client.MessageReceived += HandleCommandAsync;

            await StaticObjects.Client.LoginAsync(TokenType.Bot, credentials.BotToken);
            await StaticObjects.Client.StartAsync();

            // We keep the bot online
            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage msg || msg.Content == null || msg.Content == "") return; // The message received isn't one we can deal with

            // Deprecation warning
            int pos = 0;
            if (msg.HasMentionPrefix(StaticObjects.Client.CurrentUser, ref pos))
            {
                var content = msg.Content[pos..];
                var commandStr = content.Split(' ')[0].ToUpperInvariant();
            }
        }
    }
}