using System;
using System.Collections.Generic;
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
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int MaxMana { get; set; }
    public int CurrentMana { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }

    public Texture2D? CharacterImage { get; set; }

    public CharacterNode Node { get; set; }

    private readonly List<Buff> buffs = new();

    /// <summary>
    /// Number of turns this character has already taken.
    /// </summary>
    public int Round { get; private set; }

    public CharacterData(string name, int maxHP, int maxMana, int speed, int attack, int defence, Texture2D? characterImage = null)
    {
        Name = name;
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

    /// <summary>
    /// Returns the effective speed for the specified round.
    /// Currently this is the base speed but will later include buffs or debuffs.
    /// </summary>
    public int GetSpeed(int round) => GetStat(CharacterStat.Speed, round);

    public int GetStat(CharacterStat stat, int round)
    {
        int baseValue = stat switch
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

    public int GetCurrentStat(CharacterStat stat) => GetStat(stat, Round);

    /// <summary>
    /// Returns the effective speed for the current round.
    /// </summary>
    public int GetCurrentSpeed() => GetSpeed(Round);

    public int GetAttack(int round) => GetStat(CharacterStat.Attack, round);

    public int GetCurrentAttack() => GetAttack(Round);

    public int GetDefence(int round) => GetStat(CharacterStat.Defence, round);

    public int GetCurrentDefence() => GetDefence(Round);

    /// <summary>
    /// Increments the internal round counter.
    /// </summary>
    public void AdvanceRound() => Round++;

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
    }

    public void RemoveExpiredBuffs(int round)
    {
        buffs.RemoveAll(b => !b.IsActive(round));
    }
   
}