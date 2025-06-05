using Xunit;

public class CharacterDataTests
{
    [Theory]
    [InlineData(1, true, false)]
    [InlineData(0, false, true)]
    [InlineData(-5, false, true)]
    public void IsAlive_And_IsDead_Work_AsExpected(int hp, bool expectedAlive, bool expectedDead)
    {
        var character = new CharacterData("Test", 10, 5, 1, 1, 1);
        character.CurrentHP = hp;

        Assert.Equal(expectedAlive, character.IsAlive);
        Assert.Equal(expectedDead, character.IsDead);
    }

    [Fact]
    public void GetSpeed_Returns_BaseSpeed()
    {
        var character = new CharacterData("Test", 10, 5, 7, 1, 1);
        Assert.Equal(7, character.GetSpeed(0));
    }

    [Fact]
    public void AdvanceRound_Increments_Round()
    {
        var character = new CharacterData("Test", 10, 5, 7, 1, 1);
        character.AdvanceRound();
        Assert.Equal(1, character.Round);
    }
}
