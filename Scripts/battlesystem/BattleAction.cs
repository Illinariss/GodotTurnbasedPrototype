
using System.Dynamic;

public class BattleAction
{
    public BattleAction(BasicAbility ability, CharacterData target)
    {
        this.Ability = ability;
        this.Target = target;
    }
    public BasicAbility Ability { get; set; }
    public CharacterData Target { get; set; }

}