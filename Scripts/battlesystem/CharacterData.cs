using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public enum CharacterStat
{
    Speed,
    Attack,
    Defence,
    MaxHP,
    CurrentHP,
    MaxMana,
    CurrentMana
}

public class CharacterData
{
    public string Name { get; set; }
    public float MaxHP { get; set; }
    public float CurrentHP { get; set; }
    public float MaxMana { get; set; }
    public float CurrentMana { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }

    public Texture2D? CharacterImage { get; set; }

    public CharacterNode Node { get; set; }

    public ICombatAI? CombatAI
    {
        get => combatAI;
        set
        {
            combatAI = value;
            combatAI?.Initialize(this);
        }
    }

    private ICombatAI? combatAI;

    public List<BasicAbility> Abilities { get; } = new();

    private readonly List<Buff> buffs = new();

    /// <summary>
    /// Number of turns this character has already taken.
    /// </summary>
    public int Turn { get; private set; }

    public CharacterData(string name, bool isPlayerCharacter, int maxHP, int maxMana, int speed, int attack, int defence, Texture2D? characterImage = null)
    {
        Name = name;
        IsPlayerCharacter = isPlayerCharacter;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        MaxMana = maxMana;
        CurrentMana = maxMana;
        Speed = speed;
        Attack = attack;
        Defence = defence;
        CharacterImage = characterImage;
    }

    public bool IsAlive => CurrentHP > 0;

    public bool IsDead => CurrentHP <= 0;

    public bool IsPlayerCharacter { get; internal set; }

    public float GetStat(CharacterStat stat, int round)
    {
        float baseValue = stat switch
        {
            CharacterStat.Speed => Speed,
            CharacterStat.Attack => Attack,
            CharacterStat.Defence => Defence,
            CharacterStat.MaxHP => MaxHP,
            CharacterStat.CurrentHP => CurrentHP,
            CharacterStat.MaxMana => MaxMana,
            CharacterStat.CurrentMana => CurrentMana,
            _ => 0
        };

        var bonus = 0f;
        foreach (var buff in buffs)
        {
            if (buff.TargetStat == stat && buff.IsActive(round))
            {
                bonus += baseValue * buff.Factor + buff.Value;
            }
        }

        return (int)MathF.Round(baseValue + bonus);
    }

    /// <summary>
    /// Increments the internal round counter.
    /// </summary>
    public void AdvanceRound() => Turn++;

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
    }

    public void RemoveExpiredBuffs(int round)
    {
        buffs.RemoveAll(b => !b.IsActive(round));
    }

    public void RecieveDmg(float dmg, RichTextLabel log)
    {
        var afterdefense = dmg - GetStat(CharacterStat.Defence, Turn);
        if (afterdefense <= 0)
        {
            afterdefense = 1;
        }
        log?.AddText($"\nDefence is { GetStat(CharacterStat.Defence, Turn)}");
        log?.AddText($"\nDmg afer defence is {afterdefense}");
        this.CurrentHP -= afterdefense;
        log?.AddText($"\nHp is now {CurrentHP}");
    }

}