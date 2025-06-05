using Godot;
using System;
using System.Collections.Generic;

public partial class BattleScene : Control
{
    [Export] public Node2D Map;    
    [Export] public RichTextLabel BattleLog;
    [Export] public Button NextTurnButton;

    private List<CharacterNode> playerNodes = new();
    private List<CharacterNode> enemyNodes = new();
    private BattleManager battleManager;
    
     private Action onBattleFinished;
    public void Setup(List<CharacterData> playerCharacters, List<CharacterData> enemies, Action onFinishedCallback)
    {
        this.onBattleFinished = onFinishedCallback;
        foreach (var playerCharacter in playerCharacters)
        {
            AddCharacterNode(playerCharacter, isPlayer: true);
        }
        foreach (var enemy in enemies)
        {
            AddCharacterNode(enemy, isPlayer: false);
        }
        battleManager = new BattleManager(playerCharacters, enemies, BattleLog, onFinishedCallback);
    }
     private void AddCharacterNode(CharacterData data, bool isPlayer)
    {
        var node = CharacterNode.Create(data);
        Vector2 areaMin, areaMax;
        if (isPlayer)
        {
            areaMin = new Vector2(50, 100);
            areaMax = new Vector2(250, 600);
            Map.AddChild(node);
            playerNodes.Add(node);
        }
        else
        {
            areaMin = new Vector2(800, 100);
            areaMax = new Vector2(1100, 600);
            Map.AddChild(node);
            enemyNodes.Add(node);
        }
        var rand = new Random();
        var pos = new Vector2(
            (float)(areaMin.X + rand.NextDouble() * (areaMax.X - areaMin.X)),
            (float)(areaMin.Y + rand.NextDouble() * (areaMax.Y - areaMin.Y))
        );
        node.Position = pos;
    }


    public override void _Ready()
    {

    }

    private void OnNextTurn()
    {
        battleManager.ExecuteTurn();
        // Optional: UI aktualisieren
        foreach (var node in playerNodes) node.UpdateUI();
        foreach (var node in enemyNodes) node.UpdateUI();
    }
}
