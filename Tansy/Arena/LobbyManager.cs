namespace Tansy.Arena
{
    public class LobbyManager
    {
        public bool Has(ulong channelId)
            => _lobbies.ContainsKey(channelId);

        public void Add(ulong channelId, Lobby lobby)
            => _lobbies.Add(channelId, lobby);

        public void Remove(ulong channelId)
            => _lobbies.Remove(channelId);

        private readonly Dictionary<ulong, Lobby> _lobbies = new();
    }
}
