public class AttackAbility : BasicAbility
{
    private readonly System.Random random = new();
    public AttackAbility()
    {
        Type = AbilityType.Attack;
        Scalestat = CharacterStat.Attack;
        Faktor = 1;
        ResourceCost = 0;
        Name = "Attack";
        RequiresTarget = true;
   }

    /// <summary>
    /// Calculates damage with a random multiplier between 0.75 and 1.25.
    /// </summary>
    public override float CalculateDamage(CharacterData attacker)
    {
        float variance = (float)(random.NextDouble() * 0.5 + 0.75); // 0.75 - 1.25
        return Faktor * attacker.GetStat(Scalestat, attacker.Turn) * variance;
    }
}
