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
        actor.AdvanceRound();

        log.AppendText($"\n{actor.Name} ist am Zug.");

        UpcomingTurns = PredictTurns(10);
    }
}
