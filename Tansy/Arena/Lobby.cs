using Discord;

namespace Tansy.Arena
{
    public class Lobby
    {
        public Lobby(IUser owner)
        {
            _initTime = DateTime.UtcNow;
            _users.Add(owner);
        }

        public Embed GetEmbed()
        {
            return new EmbedBuilder
            {
                Title = "Lobby",
                Description = string.Join("\n", _users.Select(x => x.Mention))
                + $"\n\nStarts <t:{(int)(_initTime.AddSeconds(ReadyUpDuration) - new DateTime(1970, 1, 1)).TotalSeconds}:R>"
            }.Build();
        }

        public string GetStartMessage()
        {
            return $"{string.Join(" ", _users.Select(x => x.Mention))} the game is starting";
        }

        public bool HasUser(ulong userId)
            => _users.Any(x => x.Id == userId);

        public void Join(IUser user)
            => _users.Add(user);

        public List<IUser> Users => _users;

        public IUserMessage Message { set; get; }
        private readonly List<IUser> _users = new();

        private readonly DateTime _initTime;
        public static int ReadyUpDuration => 30;
    }
}
