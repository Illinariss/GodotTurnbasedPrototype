public enum AbilityType
{
    Attack,
    Support,
    Passive,
    Flee
}

public interface IAbility
{
    string Name { get; }
    AbilityType Type { get; }
    int Damage { get; }
    int ResourceCost { get; }
}
