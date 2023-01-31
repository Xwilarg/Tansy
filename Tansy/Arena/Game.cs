using Discord;
using System.Text;
using System.Text.RegularExpressions;
using Tansy.Arena.Fighter;

namespace Tansy.Arena
{
    public partial class Game
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
                _users = users.Select(x => new ArenaUser(x)).OrderBy(x => StaticObjects.Random.Next());
            }

            _globalLog.AppendLine("**Round 1**");
            _turnStartTime = DateTime.UtcNow;
            _statusMessage = channel.SendMessageAsync(embed: GetStatusMessage()).GetAwaiter().GetResult();

            System.Timers.Timer timer = new();
            timer.Elapsed += async (sender, _) =>
            {
                foreach (var user in _users)
                {
                    if (user.IsAlive)
                    {
                        _globalLog.AppendLine(user.Resolve());
                    }
                    user.EndTurn();
                }
                _round++;
                if (_round < _maxRound)
                {
                    _globalLog.AppendLine();
                    _globalLog.AppendLine($"**Round {(_round + 1)}**");
                    _turnStartTime = DateTime.UtcNow;
                    await _statusMessage.ModifyAsync(x => x.Embed = GetStatusMessage());
                }
                else
                {
                    await _statusMessage.ModifyAsync(x => x.Embed = GetEndEmbed());
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
                    },
                    new()
                    {
                        Name = "Pending Actions",
                        Value = string.Join("\n", _users.Select(x => $"{x.Name}: {(x.IsActionDone ? "✅" : "❌")}")),
                        IsInline = false
                    },
                    new()
                    {
                        Name = "Log",
                        Value = _globalLog.ToString(),
                        IsInline = false
                    }
                }
            }.Build();
        }

        private Embed GetEndEmbed()
        {
            return new EmbedBuilder
            {
                Title = $"Game Ended",
                Fields = new()
                {
                    new()
                    {
                        Name = "Log",
                        Value = _globalLog.ToString(),
                        IsInline = false
                    }
                },
                Color = Color.Green
            }.Build();
        }

        private ArenaUser? Parse(string id)
        {
            var m = UserRegex().Match(id);
            if (m.Success)
            {
                return _users.FirstOrDefault(x => x.Is(m.Groups[1].Value));
            }
            return _users.FirstOrDefault(x => x.Is(id));
        }

        public async Task ParseActionAsync(IUser user, string command)
        {
            var me = Parse($"{user.Id}");
            if (me == null)
            {
                return; // User not in arena
            }
            var s = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (s.Length != 2)
            {
                return; // Invalid number of arguments
            }
            switch (s[0])
            {
                case "ATTACK":
                    var target = Parse(s[1]);
                    if (target == null)
                    {
                        return; // Invalid target
                    }
                    me.SetAction(ActionType.ATTACK, target);
                    await _statusMessage.ModifyAsync(x => x.Embed = GetStatusMessage());
                    break;
            }
        }

        private IUserMessage _statusMessage;
        private int _round, _maxRound = 3;
        private ITextChannel _channel;
        private IEnumerable<ArenaUser> _users;

        private DateTime _turnStartTime;

        private const int _turnDuration = 10000;

        private StringBuilder _globalLog = new();

        [GeneratedRegex("<@([0-9]+)>")]
        private static partial Regex UserRegex();
    }
}
