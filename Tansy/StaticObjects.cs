using Discord.WebSocket;
using Discord;
using Tansy.Arena;

namespace Tansy
{
    public static class StaticObjects
    {
        public static DiscordSocketClient Client { get; } = new(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        });

        public static Random Random { get; } = new();

        public static LobbyManager LobbyManager { get; } = new();

        private static int _id = 0;
        public static int UniqueId
        {
            get
            {
                return _id++;
            }
        }
    }
}
