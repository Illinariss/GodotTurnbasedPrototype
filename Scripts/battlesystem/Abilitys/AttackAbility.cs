public class AttackAbility : BasicAbility
{
    public AttackAbility()
    {
        Type = AbilityType.Attack;
        Scalestat = CharacterStat.Attack;
        Faktor = 1;
        ResourceCost = 0;
        Name = "Attack";
        RequiresTarget = true;
   }
}