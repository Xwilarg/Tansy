using Discord;
using Tansy.Arena.Fighter;

namespace Tansy.Arena
{
    public class Game
    {
        public Game(ITextChannel channel, List<IUser> users)
        {
            _channel = channel;
            if (users.Count == 1) // Only one user, we add a training dummy
            {
                _users = new ArenaUser[] { new(users[0]), new("DUMMY") };
            }
            else
            {
                _users = users.Select(x => new ArenaUser(x));
            }

            _statusMessage = channel.SendMessageAsync(embed: GetStatusMessage()).GetAwaiter().GetResult();

            System.Timers.Timer timer = new();
            timer.Elapsed += async (sender, _) =>
            {
                _round++;
                if (_round < _maxRound)
                {
                    await _statusMessage.ModifyAsync(x => x.Embed = GetStatusMessage());
                }
                else
                {
                    await _channel.SendMessageAsync("End of test, deleting lobby");
                    StaticObjects.GameManager.Remove(_channel.Id);
                }
            };
            timer.Interval = 2000;
            timer.Enabled = true;
        }

        private Embed GetStatusMessage()
        {
            return new EmbedBuilder
            {
                Title = $"Round {_round + 1} / {_maxRound}",
                Description = string.Join("\n", _users)
            }.Build();
        }

        private IUserMessage _statusMessage;
        private int _round, _maxRound = 3;
        private ITextChannel _channel;
        private IEnumerable<ArenaUser> _users;
    }
}
