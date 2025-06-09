using System;
using System.Linq;

public class RandomCombatAI : ICombatAI
{
    private CharacterData self;
    private readonly Random random = new Random();

    public void Initialize(CharacterData self)
    {
        this.self = self;
    }

    public BattleAction DecideNextAction(BattleContext context)
    {
        var damagingAbilities = self?.Abilities
            .Where(a => a.Type == AbilityType.Attack && a.Damage > 0)
            .ToList();

        if (damagingAbilities == null || damagingAbilities.Count == 0)
        {
            return new BattleAction { SelectedAktion = string.Empty };
        }

        var ability = damagingAbilities[random.Next(damagingAbilities.Count)];
        return new BattleAction { SelectedAktion = ability.Name };
    }
}
