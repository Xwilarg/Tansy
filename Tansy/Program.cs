using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text.Json;
using Tansy.Arena;

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
                var content = msg.Content[pos..].ToUpperInvariant();
                var commandStr = content.Split(' ')[0];

                switch (commandStr)
                {
                    case "START":
                        if (StaticObjects.GameManager.Has(msg.Channel.Id))
                        {
                            await msg.Channel.SendMessageAsync("A game is already running", messageReference: new(msg.Id));
                        }
                        else if (StaticObjects.LobbyManager.Has(msg.Channel.Id))
                        {
                            await msg.Channel.SendMessageAsync("There is already a pending lobby", messageReference: new(msg.Id));
                        }
                        else
                        {
                            var lobby = new Lobby(msg.Author);
                            StaticObjects.LobbyManager.Add(msg.Channel.Id, lobby);
                            lobby.Message = await msg.Channel.SendMessageAsync(embed: lobby.GetEmbed());
                        }
                        break;

                    case "JOIN":
                        if (StaticObjects.GameManager.Has(msg.Channel.Id))
                        {
                            await msg.Channel.SendMessageAsync("A game is already running", messageReference: new(msg.Id));
                        }
                        else if (!StaticObjects.LobbyManager.Has(msg.Channel.Id))
                        {
                            await msg.Channel.SendMessageAsync("There is no pending lobby", messageReference: new(msg.Id));
                        }
                        else
                        {
                            var lobby = StaticObjects.LobbyManager.Get(msg.Channel.Id);
                            if (lobby.HasUser(msg.Author.Id))
                            {
                                await msg.Channel.SendMessageAsync("You already joined this lobby", messageReference: new(msg.Id));
                            }
                            else
                            {
                                lobby.Join(msg.Author);
                                await lobby.Message.ModifyAsync(x => x.Embed = lobby.GetEmbed());
                            }
                        }
                        break;

                    default:
                        if (StaticObjects.GameManager.Has(msg.Channel.Id))
                        {
                            await StaticObjects.GameManager.Get(msg.Channel.Id).ParseActionAsync(msg.Author, content);
                        }
                        break;
                }

                await msg.DeleteAsync();
            }
        }
    }
}