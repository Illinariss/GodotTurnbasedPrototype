using Xunit;

public class AttackAbilityTests
{
    [Fact]
    public void CalculateDamage_WithinExpectedRange()
    {
        var attacker = new CharacterData("Attacker", true, 100, 0, 10, 20, 5);
        var ability = new AttackAbility();
        var baseDmg = attacker.GetStat(CharacterStat.Attack, attacker.Turn);
        for (int i = 0; i < 20; i++)
        {
            var dmg = ability.CalculateDamage(attacker);
            Assert.InRange(dmg, 0.75f * baseDmg, 1.25f * baseDmg);
        }
    }
}
