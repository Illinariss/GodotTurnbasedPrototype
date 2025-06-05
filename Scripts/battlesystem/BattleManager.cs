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
    private int currentEnemyIndex = 0;
    private bool playerTurn = true;

    public BattleManager(List<CharacterData> playerCharacters, List<CharacterData> enemies, RichTextLabel log, Action onFinishedCallback)
    {
        this.playerCharacters = playerCharacters;
        this.enemies = enemies;
        this.log = log;
        this.onFinishedCallback = onFinishedCallback;
    }

    public void CalculateTurnOrder()
    {
        var chars = new List<CharacterData>();
    }

    public void ExecuteTurn()
    {

        if (playerCharacters.All(p => p.IsDead) || enemies.All(p => p.IsDead))
        {
            log.AppendText("\nDer Kampf ist vorbei.");
            return;
        }


        // if (playerTurn)
        // {
        //     var target = enemies[currentEnemyIndex % enemies.Count];
        //     int damage = Mathf.Max(0, playerCharacters.Attack - target.Defence);
        //     target.CurrentHP -= damage;
        //     log.AppendText($"\n{playerCharacters.Name} greift {target.Name} an für {damage} Schaden!");
        // }
        // else
        // {
        //     var attacker = enemies[currentEnemyIndex % enemies.Count];
        //     int damage = Mathf.Max(0, attacker.Attack - playerCharacters.Defence);
        //     playerCharacters.CurrentHP -= damage;
        //     log.AppendText($"\n{attacker.Name} greift {playerCharacters.Name} an für {damage} Schaden!");
        // }

        playerTurn = !playerTurn;
        if (!playerTurn)
            currentEnemyIndex++;
    }
}
