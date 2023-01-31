namespace Tansy.Arena.Equipement
{
    public record Weapon
    {
        public required string Name { init; get; }
        public required int Damage { init; get; }
    }
}
