using Discord;
using Tansy.Arena.Equipement;

namespace Tansy.Arena.Fighter
{
    public class ArenaUser
    {
        public ArenaUser(IUser user)
        {
            _id = $"{user.Id}";
            _nickname = user.Mention;
        }

        public ArenaUser(string id)
        {
            _id = id;
            _nickname = null;
        }

        public string Name => _nickname ?? _id;

        public override string ToString()
        {
            return $"{Name}: {_health} HP";
        }

        public void SetAction(ActionType action, object arg)
        {
            _action = action;
            _arg = arg;
        }

        public string Resolve()
        {
            switch (_action)
            {
                case ActionType.ATTACK:
                    var target = (ArenaUser)_arg;
                    target.TakeDamage(_defaultWeapon);
                    return $"{Name} attacks {target.Name} with {_defaultWeapon.Name.ToLowerInvariant()} for {_defaultWeapon.Damage} damage";

                case ActionType.NONE:
                    return $"{Name} do nothing";
            }
            throw new NotImplementedException();
        }

        public void EndTurn()
        {
            _action = ActionType.NONE;
        }

        public bool Is(string id)
            => _id == id;

        public bool IsAlive => _health > 0;

        public void TakeDamage(Weapon weapon)
        {
            _health -= weapon.Damage;
        }

        public bool IsActionDone => _id == "DUMMY" || _action != ActionType.NONE;

        private string _id;
        private string? _nickname;
        private int _health = 100;

        private ActionType _action;
        private object _arg;

        private static Weapon _defaultWeapon = new()
        {
            Name = "Fists",
            Damage = 10
        };
    }
}
