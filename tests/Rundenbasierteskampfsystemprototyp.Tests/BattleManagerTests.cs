using System.Collections.Generic;
using System.Linq;
using Xunit;

public class BattleManagerTests
{
    [Fact]
    public void PredictTurns_Uses_Speed_For_Order()
    {
        var a = new CharacterData("A",true, 10, 5, 10, 1, 1);
        var b = new CharacterData("B",false, 10, 5, 5, 1, 1);
        var manager = new BattleManager(new List<CharacterData>{a}, new List<CharacterData>{b}, null!, () => { });
        var order = manager.UpcomingTurns.Take(6).ToList();
        Assert.Equal(new[]{a, a, b, a, a, b}, order);
    }

    private class DummyAI : ICombatAI
    {
        public bool DecideCalled { get; private set; }
        public void Initialize(CharacterData self) { }
        public BattleAction DecideNextAction(BattleContext context)
        {
            DecideCalled = true;
            return new BattleAction();
        }
    }

    [Fact]
    public void ExecuteTurn_Calls_AI_For_Enemy()
    {
        var player = new CharacterData("P", true, 10, 5, 1, 1, 1);
        var enemy = new CharacterData("E", false, 10, 5, 100, 1, 1);
        var ai = new DummyAI();
        enemy.CombatAI = ai;

        var manager = new BattleManager(new List<CharacterData>{player}, new List<CharacterData>{enemy}, null!, () => { });

        manager.ExecuteTurn();

        Assert.True(ai.DecideCalled);
    }
}
