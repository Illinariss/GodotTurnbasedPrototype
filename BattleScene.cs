using Godot;
using System;
using System.Collections.Generic;

public partial class BattleScene : Control
{
    [Export] public Node2D Map;
    [Export] public RichTextLabel BattleLog;
    [Export] public Button NextTurnButton;
    [Export] public HBoxContainer TurnOrderContainer;

    private List<CharacterNode> playerNodes = new();
    private List<CharacterNode> enemyNodes = new();
    private BattleManager battleManager;
    private Queue<Vector2> playerPositions = new();
    private Queue<Vector2> enemyPositions = new();
    
     private Action onBattleFinished;
    public void Setup(List<CharacterData> playerCharacters, List<CharacterData> enemies, Action onFinishedCallback)
    {
        this.onBattleFinished = onFinishedCallback;

        playerPositions = new Queue<Vector2>(GetGridPositions(playerCharacters.Count, new Vector2(50, 100), new Vector2(250, 400)));
        enemyPositions = new Queue<Vector2>(GetGridPositions(enemies.Count, new Vector2(800, 100), new Vector2(1100, 400)));

        foreach (var playerCharacter in playerCharacters)
        {
            AddCharacterNode(playerCharacter, isPlayer: true);
        }
        foreach (var enemy in enemies)
        {
            AddCharacterNode(enemy, isPlayer: false);
        }
        battleManager = new BattleManager(playerCharacters, enemies, BattleLog, onFinishedCallback);
        UpdateTurnOrderDisplay();
    }

    private List<Vector2> GetGridPositions(int count, Vector2 areaMin, Vector2 areaMax)
    {
        var result = new List<Vector2>();
        if (count <= 0)
            return result;

        if (count == 1)
        {
            result.Add((areaMin + areaMax) / 2);
            return result;
        }

        if (count == 2)
        {
            var x = (areaMin.X + areaMax.X) / 2f;
            var cellHeight = (areaMax.Y - areaMin.Y) / 2f;
            result.Add(new Vector2(x, areaMin.Y + cellHeight / 2f));
            result.Add(new Vector2(x, areaMin.Y + 3f * cellHeight / 2f));
            return result;
        }

        int columns = (int)Math.Ceiling(Math.Sqrt(count));
        int rows = (int)Math.Ceiling((float)count / columns);

        float cellWidth = (areaMax.X - areaMin.X) / columns;
        float cellHeight2 = (areaMax.Y - areaMin.Y) / rows;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                result.Add(new Vector2(
                    areaMin.X + c * cellWidth + cellWidth / 2f,
                    areaMin.Y + r * cellHeight2 + cellHeight2 / 2f));
                if (result.Count == count)
                    return result;
            }
        }

        return result;
    }

    private void AddCharacterNode(CharacterData data, bool isPlayer)
    {
        var node = CharacterNode.Create(data);
        List<CharacterNode> targetList;
        Vector2 pos;
        if (isPlayer)
        {
            targetList = playerNodes;
            pos = playerPositions.Count > 0 ? playerPositions.Dequeue() : Vector2.Zero;
        }
        else
        {
            targetList = enemyNodes;
            pos = enemyPositions.Count > 0 ? enemyPositions.Dequeue() : Vector2.Zero;
        }

        node.Position = pos;

        Map.AddChild(node);
        targetList.Add(node);
    }


    public override void _Ready()
    {
        if (Map == null)
        {
            // Assign the Battlers node if not linked via the inspector
            Map = GetNode<Node2D>("%Battlers");
        }

        if (NextTurnButton != null)
            NextTurnButton.Pressed += OnNextTurn;

        if (TurnOrderContainer == null)
            TurnOrderContainer = GetNode<HBoxContainer>("%TurnOrderContainer");
    }

    private void OnNextTurn()
    {
        battleManager.ExecuteTurn();
        // Optional: UI aktualisieren
        foreach (var node in playerNodes) node.UpdateUI();
        foreach (var node in enemyNodes) node.UpdateUI();
        UpdateTurnOrderDisplay();
    }

    private void UpdateTurnOrderDisplay()
    {
        if (TurnOrderContainer == null || battleManager == null)
            return;

        foreach (Node child in TurnOrderContainer.GetChildren())
            child.QueueFree();

        var turns = battleManager.UpcomingTurns;
        for (int i = 0; i < turns.Count; i++)
        {
            var btn = new Button();
            btn.Text = $"{i + 1}. {turns[i].Name}";
            TurnOrderContainer.AddChild(btn);
        }
    }
}
