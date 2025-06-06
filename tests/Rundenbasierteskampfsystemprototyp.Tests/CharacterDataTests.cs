using Xunit;

public class CharacterDataTests
{
    [Fact]
    public void Name_IsStored()
    {
        var character = new CharacterData("TestName", 10, 5, 1, 1, 1);
        Assert.Equal("TestName", character.Name);
    }

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
}
