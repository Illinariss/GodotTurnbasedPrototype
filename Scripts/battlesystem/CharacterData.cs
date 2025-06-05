public class CharacterData
{
    public string Name { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int MaxMana { get; set; }
    public int CurrentMana { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }

    public CharacterNode Node { get; set; }

    /// <summary>
    /// Number of turns this character has already taken.
    /// </summary>
    public int Round { get; private set; }

    public CharacterData(string name, int maxHP, int maxMana, int speed, int attack, int defence)
    {
        Name = name;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        MaxMana = maxMana;
        CurrentMana = maxMana;
        Speed = speed;
        Attack = attack;
        Defence = defence;
    }

    public bool IsAlive => CurrentHP > 0;

    public bool IsDead => CurrentHP <= 0;

    /// <summary>
    /// Returns the effective speed for the specified round.
    /// Currently this is the base speed but will later include buffs or debuffs.
    /// </summary>
    public int GetSpeed(int round) => Speed;

    /// <summary>
    /// Returns the effective speed for the current round.
    /// </summary>
    public int GetCurrentSpeed() => GetSpeed(Round);

    /// <summary>
    /// Increments the internal round counter.
    /// </summary>
    public void AdvanceRound() => Round++;
   
}