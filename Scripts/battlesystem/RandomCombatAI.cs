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
            .Where(a => a.Type == AbilityType.Attack)
            .ToList();

        if (damagingAbilities == null || damagingAbilities.Count == 0)
        {
            throw new Exception($"Character {self.Name} has no abbilitys!");
        }
        if (context.Enemylist == null || context.Enemylist.Count == 0)
        {
            throw new Exception($"Character {self.Name} has no targets!");
        }
        var ability = damagingAbilities[random.Next(damagingAbilities.Count)];
        var target = context.Enemylist[random.Next(context.Enemylist.Count)];
        return new BattleAction(ability,target);
    }
}
