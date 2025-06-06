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
}
