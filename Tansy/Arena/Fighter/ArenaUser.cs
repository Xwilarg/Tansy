using Discord;

namespace Tansy.Arena.Fighter
{
    public class ArenaUser
    {
        public ArenaUser(IUser user)
        {
            _id = user.Mention;
        }

        public ArenaUser(string id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return $"{_id}: {_health} HP";
        }

        private string _id;
        private int _health = 100;
    }
}
