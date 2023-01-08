using Discord;

namespace Tansy.Arena
{
    public class GameManager
    {
        public bool Has(ulong channelId)
            => _games.ContainsKey(channelId);

        public void Add(ulong channelId, Game game)
            => _games.Add(channelId, game);

        public void Remove(ulong channelId)
            => _games.Remove(channelId);

        private readonly Dictionary<ulong, Game> _games = new();
    }
}
