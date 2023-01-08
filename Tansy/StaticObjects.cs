using Discord.WebSocket;
using Discord;

namespace Tansy
{
    public static class StaticObjects
    {
        public static DiscordSocketClient Client { get; } = new(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        });
    }
}
