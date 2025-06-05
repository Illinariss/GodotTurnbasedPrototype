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
    private bool IsOverlapping(Vector2 position, List<CharacterNode> nodes, float minDistance)
    {
        foreach (var n in nodes)
        {
            if (n.Position.DistanceTo(position) < minDistance)
                return true;
        }
        return false;
    }

    private Vector2 GetRandomPosition(Vector2 areaMin, Vector2 areaMax, List<CharacterNode> existingNodes)
    {
        var rand = new Random();
        const int maxAttempts = 100;
        const float minDistance = 80f;

        for (int i = 0; i < maxAttempts; i++)
        {
            var pos = new Vector2(
                (float)(areaMin.X + rand.NextDouble() * (areaMax.X - areaMin.X)),
                (float)(areaMin.Y + rand.NextDouble() * (areaMax.Y - areaMin.Y))
            );

            if (!IsOverlapping(pos, existingNodes, minDistance))
                return pos;
        }

        // Fallback if no free position was found
        return new Vector2(
            (float)(areaMin.X + rand.NextDouble() * (areaMax.X - areaMin.X)),
            (float)(areaMin.Y + rand.NextDouble() * (areaMax.Y - areaMin.Y))
        );
    }

    private void AddCharacterNode(CharacterData data, bool isPlayer)
    {
        var node = CharacterNode.Create(data);
        Vector2 areaMin, areaMax;
        List<CharacterNode> targetList;
        if (isPlayer)
        {
            areaMin = new Vector2(50, 100);
            areaMax = new Vector2(250, 600);
            targetList = playerNodes;
        }
        else
        {
            areaMin = new Vector2(800, 100);
            areaMax = new Vector2(1100, 600);
            targetList = enemyNodes;
        }

        var pos = GetRandomPosition(areaMin, areaMax, targetList);
        node.Position = pos;

        Map.AddChild(node);
        targetList.Add(node);
    }


    public override void _Ready()
    {
        if (NextTurnButton != null)
            NextTurnButton.Pressed += OnNextTurn;
    }

    private void OnNextTurn()
    {
        battleManager.ExecuteTurn();
        // Optional: UI aktualisieren
        foreach (var node in playerNodes) node.UpdateUI();
        foreach (var node in enemyNodes) node.UpdateUI();
    }
}
