
using System.Dynamic;

public class BattleAction
{
    public BattleAction(BasicAbility ability, CharacterData? target = null)
    {
        Ability = ability;
        Target = target;
    }

    public BasicAbility Ability { get; set; }
    public CharacterData? Target { get; set; }

}