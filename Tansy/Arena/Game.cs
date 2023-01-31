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

            _turnStartTime = DateTime.UtcNow;
            _statusMessage = channel.SendMessageAsync(embed: GetStatusMessage()).GetAwaiter().GetResult();

            System.Timers.Timer timer = new();
            timer.Elapsed += async (sender, _) =>
            {
                _round++;
                if (_round < _maxRound)
                {
                    _turnStartTime = DateTime.UtcNow;
                    await _statusMessage.ModifyAsync(x => x.Embed = GetStatusMessage());
                }
                else
                {
                    await _statusMessage.ModifyAsync(x => x.Embed = new EmbedBuilder
                    {
                        Title = "Game ended",
                        Color = Color.Green
                    }.Build());
                    StaticObjects.GameManager.Remove(_channel.Id);
                    timer.Enabled = false;
                }
            };
            timer.Interval = _turnDuration;
            timer.Enabled = true;
        }

        private Embed GetStatusMessage()
        {
            return new EmbedBuilder
            {
                Title = $"Round {_round + 1} / {_maxRound}",
                Description = string.Join("\n", _users),
                Fields = new()
                {
                    new()
                    {
                        Name = "Turn ends",
                        Value = $"<t:{(int)(_turnStartTime.AddSeconds(_turnDuration / 1000) - new DateTime(1970, 1, 1)).TotalSeconds}:R>",
                        IsInline = true
                    }
                }
            }.Build();
        }

        private IUserMessage _statusMessage;
        private int _round, _maxRound = 3;
        private ITextChannel _channel;
        private IEnumerable<ArenaUser> _users;

        private DateTime _turnStartTime;

        private const int _turnDuration = 10000;
    }
}
