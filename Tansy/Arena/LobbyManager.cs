namespace Tansy.Arena
{
    public class LobbyManager
    {
        public bool Has(ulong channelId)
            => _lobbies.ContainsKey(channelId);

        public void Add(ulong channelId, Lobby lobby)
        {
            System.Timers.Timer timer = new();
            timer.Elapsed += async (sender, _) =>
            {
                var targetChannel = lobby.Message.Channel;
                await lobby.Message.DeleteAsync();
                var startMsg = await targetChannel.SendMessageAsync(lobby.GetStartMessage());
                await Task.Delay(2000);
                await startMsg.DeleteAsync();
                _lobbies.Remove(channelId);
            };
            timer.Interval = Lobby.ReadyUpDuration;
            timer.Enabled = true;
            _lobbies.Add(channelId, lobby);
        }

        public Lobby Get(ulong channelId)
            => _lobbies[channelId];

        private readonly Dictionary<ulong, Lobby> _lobbies = new();
    }
}
