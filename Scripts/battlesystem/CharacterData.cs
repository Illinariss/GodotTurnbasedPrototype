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
   
}