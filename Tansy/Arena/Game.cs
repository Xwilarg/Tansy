using Discord;

namespace Tansy.Arena
{
    public class Game
    {
        public Game(ITextChannel channel, List<IUser> users)
        {
            _channel = channel;
            _users = users;
            System.Timers.Timer timer = new();
            timer.Elapsed += async (sender, _) =>
            {
                await _channel.SendMessageAsync("End of test, deleting lobby");
                StaticObjects.GameManager.Remove(_channel.Id);
                timer.Enabled = false;
            };
            timer.Interval = 2000;
            timer.Enabled = true;
        }

        private ITextChannel _channel;
        private List<IUser> _users;
    }
}
