using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class BattleManager
{
    private List<CharacterData> playerCharacters;
    private List<CharacterData> enemies;
    private RichTextLabel log;
    private Action onFinishedCallback;
    private Dictionary<CharacterData, int> initiativeGauge = new();
    private List<CharacterData> allCharacters = new();
    private const float SkipRefundFactor = 0.75f;

    public event Func<CharacterData, BattleContext, Task<BattleAction>>? RequestPlayerAction;
    

    /// <summary>
    /// Contains the next characters that will act. Updated after each turn.
    /// </summary>
    public List<CharacterData> UpcomingTurns { get; private set; } = new();

    public BattleManager(List<CharacterData> playerCharacters, List<CharacterData> enemies, RichTextLabel log, Action onFinishedCallback)
    {
        this.playerCharacters = playerCharacters;
        this.enemies = enemies;
        this.log = log;
        this.onFinishedCallback = onFinishedCallback;

        allCharacters.AddRange(playerCharacters);
        allCharacters.AddRange(enemies);

        foreach (var ch in allCharacters)
        {
            initiativeGauge[ch] = 0;
        }

        UpcomingTurns = PredictTurns(10);
    }

    private int GetThreshold(Dictionary<CharacterData, int> gauge, Dictionary<CharacterData, int> rounds)
    {
        return allCharacters
            .Where(c => c.IsAlive)
            .Select(c => (int)c.GetStat(CharacterStat.Speed, rounds[c]))
            .DefaultIfEmpty(0)
            .Max() * 100;
    }

    private CharacterData StepTurn(Dictionary<CharacterData, int> gauge, Dictionary<CharacterData, int> rounds)
    {
        while (true)
        {
            int threshold = GetThreshold(gauge, rounds);

            foreach (var ch in allCharacters.Where(c => c.IsAlive))
            {
                gauge[ch] += (int)ch.GetStat(CharacterStat.Speed, rounds[ch]);
                if (gauge[ch] >= threshold)
                {
                    gauge[ch] -= threshold;
                    rounds[ch]++;
                    return ch;
                }
            }
        }
    }

    public void Guard(CharacterData actor)
    {
        var buff = new Buff(CharacterStat.Defence, 0.5f, 0, actor.Turn, actor.Turn + 1);
        actor.AddBuff(buff);
        log?.AppendText($"\n{actor.Name} verteidigt sich.");
    }

    public void Skip(CharacterData actor)
    {
        var rounds = allCharacters.ToDictionary(c => c, c => c.Turn);
        int threshold = GetThreshold(initiativeGauge, rounds);
        initiativeGauge[actor] += (int)(threshold * SkipRefundFactor);
        log?.AppendText($"\n{actor.Name} hält sich zurück.");
    }

    private List<CharacterData> PredictTurns(int count)
    {
        var gaugeCopy = new Dictionary<CharacterData, int>(initiativeGauge);
        var roundCopy = allCharacters.ToDictionary(c => c, c => c.Turn);
        var result = new List<CharacterData>();

        for (int i = 0; i < count; i++)
        {
            var next = StepTurn(gaugeCopy, roundCopy);
            result.Add(next);
        }

        return result;
    }

    public async Task ExecuteTurn()
    {
        if (playerCharacters.All(p => p.IsDead) || enemies.All(p => p.IsDead))
        {
            log.AppendText("\nDer Kampf ist vorbei.");
            return;
        }

        var rounds = allCharacters.ToDictionary(c => c, c => c.Turn);
        var actor = StepTurn(initiativeGauge, rounds);

        actor.RemoveExpiredBuffs(actor.Turn);
        log?.AppendText($"\n{actor.Name} ist am Zug.");

        var context = new BattleContext
        {
            Enemylist = playerCharacters.Contains(actor) ? enemies : playerCharacters
        };

        BattleAction? action = null;
        if (!actor.IsPlayerCharacter && actor.CombatAI != null)
        {
            action = actor.CombatAI.DecideNextAction(context);
        }
        else if (RequestPlayerAction != null)
        {
            action = await RequestPlayerAction.Invoke(actor, context);
        }

        if (action != null)
        {
            ExcecuteAction(actor, action);
        }

        actor.AdvanceRound();
        UpcomingTurns = PredictTurns(10);

    }

    public void ExcecuteAction(CharacterData character, BattleAction action)
    {
        var target = action.Target;
        if (action.Ability.Type == AbilityType.Attack)
        {
            var dmg = action.Ability.Faktor * character.GetStat(action.Ability.Scalestat, character.Turn);
            action.Target.RecieveDmg(dmg);
        }
    }
}
