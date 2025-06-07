using System;
using System.Collections.Generic;
using System.Linq;
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
            .Select(c => c.GetSpeed(rounds[c]))
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
                gauge[ch] += ch.GetSpeed(rounds[ch]);
                if (gauge[ch] >= threshold)
                {
                    gauge[ch] -= threshold;
                    rounds[ch]++;
                    return ch;
                }
            }
        }
    }

    public void Attack(CharacterData attacker, CharacterData target)
    {
        int atk = attacker.GetCurrentAttack();
        int def = target.GetCurrentDefence();
        int damage = Math.Max(1, atk - def);
        target.CurrentHP = Math.Max(0, target.CurrentHP - damage);
        log?.AppendText($"\n{attacker.Name} greift {target.Name} an und verursacht {damage} Schaden.");
        if (target.IsDead)
        {
            log?.AppendText($"\n{target.Name} hat keine HP mehr und wurde besiegt.");
        }
        else
        {
            log?.AppendText($"\n{target.Name} hat noch {target.CurrentHP}/{target.MaxHP} HP.");
        }
    }

    public void Guard(CharacterData actor)
    {
        var buff = new Buff(CharacterStat.Defence, 0.5f, 0, actor.Round, actor.Round + 1);
        actor.AddBuff(buff);
        log?.AppendText($"\n{actor.Name} verteidigt sich.");
    }

    public void Skip(CharacterData actor)
    {
        var rounds = allCharacters.ToDictionary(c => c, c => c.Round);
        int threshold = GetThreshold(initiativeGauge, rounds);
        initiativeGauge[actor] += (int)(threshold * SkipRefundFactor);
        log?.AppendText($"\n{actor.Name} hält sich zurück.");
    }

    private List<CharacterData> PredictTurns(int count)
    {
        var gaugeCopy = new Dictionary<CharacterData, int>(initiativeGauge);
        var roundCopy = allCharacters.ToDictionary(c => c, c => c.Round);
        var result = new List<CharacterData>();

        for (int i = 0; i < count; i++)
        {
            var next = StepTurn(gaugeCopy, roundCopy);
            result.Add(next);
        }

        return result;
    }

    public void ExecuteTurn()
    {

        if (playerCharacters.All(p => p.IsDead) || enemies.All(p => p.IsDead))
        {
            log.AppendText("\nDer Kampf ist vorbei.");
            return;
        }

        var rounds = allCharacters.ToDictionary(c => c, c => c.Round);
        var actor = StepTurn(initiativeGauge, rounds);

        actor.RemoveExpiredBuffs(actor.Round);
        log.AppendText($"\n{actor.Name} ist am Zug.");

        CharacterData target = null;
        if (playerCharacters.Contains(actor))
            target = enemies.FirstOrDefault(e => e.IsAlive);
        else
            target = playerCharacters.FirstOrDefault(p => p.IsAlive);

        if (target != null)
            Attack(actor, target);

        actor.AdvanceRound();

        UpcomingTurns = PredictTurns(10);
    }
}
