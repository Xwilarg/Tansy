using Discord;

namespace Tansy.Arena
{
    public class Lobby
    {
        public Lobby(IUser owner)
        {
            _users = new() { owner };
        }

        private List<IUser> _users;
    }
}
