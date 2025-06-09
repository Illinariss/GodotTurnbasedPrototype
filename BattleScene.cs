using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleScene : Control
{
    [Export] public Node2D Map;
    [Export] public RichTextLabel BattleLog;
    [Export] public ScrollContainer AbilityScroll;
    [Export] public GridContainer AbilityGrid;
    [Export] public HBoxContainer TurnOrderContainer;

    private List<CharacterNode> playerNodes = new();
    private List<CharacterNode> enemyNodes = new();
    private BattleManager battleManager;
    private Queue<Vector2> playerPositions = new();
    private Queue<Vector2> enemyPositions = new();

    private CharacterData currentActor;
    private BattleContext currentContext;
    private TaskCompletionSource<BattleAction>? pendingAction;
    private Button? selectedButton;
    private BasicAbility? selectedAbility;
    
     private Action onBattleFinished;
    public void Setup(List<CharacterData> playerCharacters, List<CharacterData> enemies, Action onFinishedCallback)
    {
        this.onBattleFinished = onFinishedCallback;

        playerPositions = new Queue<Vector2>(GetGridPositions(playerCharacters.Count, new Vector2(50, 50), new Vector2(250, 300)));
        enemyPositions = new Queue<Vector2>(GetGridPositions(enemies.Count, new Vector2(800, 50), new Vector2(1100, 300)));

        foreach (var playerCharacter in playerCharacters)
        {
            AddCharacterNode(playerCharacter, isPlayer: true);
        }
        foreach (var enemy in enemies)
        {
            AddCharacterNode(enemy, isPlayer: false);
        }
        battleManager = new BattleManager(playerCharacters, enemies, BattleLog, onFinishedCallback);
        battleManager.RequestPlayerAction += OnRequestPlayerAction;
        UpdateTurnOrderDisplay();
        battleManager.ExecuteTurn();
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
        node.CharacterClicked += OnCharacterClicked;
        targetList.Add(node);
    }


    public override void _Ready()
    {
        if (Map == null)
        {
            // Assign the Battlers node if not linked via the inspector
            Map = GetNode<Node2D>("%Battlers");
        }
        if (BattleLog == null)
        {
            this.BattleLog = GetNode<RichTextLabel>("%BattleLog");
            this.BattleLog.AppendText("Battle: Start");
        }
        if (AbilityScroll == null)
            AbilityScroll = GetNode<ScrollContainer>("%AbilityScroll");
        if (AbilityGrid == null)
            AbilityGrid = GetNode<GridContainer>("%AbilityGrid");
        if (TurnOrderContainer == null)
            TurnOrderContainer = GetNode<HBoxContainer>("%TurnOrderContainer");
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

    private Task<BattleAction> OnRequestPlayerAction(CharacterData actor, BattleContext context)
    {
        currentActor = actor;
        currentContext = context;
        ShowAbilityButtons(actor);
        pendingAction = new TaskCompletionSource<BattleAction>();
        return pendingAction.Task;
    }

    private void ShowAbilityButtons(CharacterData actor)
    {
        if (AbilityGrid == null) return;
        foreach (Node child in AbilityGrid.GetChildren())
            child.QueueFree();

        var abilities = new List<BasicAbility>(actor.Abilities);
        abilities.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        AbilityGrid.Columns = Math.Max(1, (int)Math.Ceiling(abilities.Count / 2.0));

        foreach (var ability in abilities)
        {
            var btn = new Button { Text = ability.Name, ToggleMode = true };
            btn.Pressed += () => OnAbilityButtonPressed(btn, ability);
            AbilityGrid.AddChild(btn);
        }
    }

    private void ClearAbilityButtons()
    {
        if (AbilityGrid == null) return;
        foreach (Node child in AbilityGrid.GetChildren())
            child.QueueFree();
    }

    private void OnAbilityButtonPressed(Button btn, BasicAbility ability)
    {
        if (pendingAction == null)
            return;

        if (selectedButton == btn)
        {
            btn.ButtonPressed = false;
            selectedButton = null;
            selectedAbility = null;
            return;
        }

        if (selectedButton != null)
            selectedButton.ButtonPressed = false;

        selectedButton = btn;
        selectedAbility = ability;
        btn.ButtonPressed = true;

        if (!ability.RequiresTarget)
        {
            FinishAction(new BattleAction(ability));
        }
    }

    private void OnCharacterClicked(CharacterNode node)
    {
        if (pendingAction == null || selectedAbility == null)
            return;

        if (!selectedAbility.RequiresTarget)
            return;

        FinishAction(new BattleAction(selectedAbility, node.Data));
    }

    private void FinishAction(BattleAction action)
    {
        pendingAction?.SetResult(action);
        pendingAction = null;

        if (selectedButton != null)
        {
            selectedButton.ButtonPressed = false;
            selectedButton = null;
        }
        selectedAbility = null;
        ClearAbilityButtons();
        foreach (var node in playerNodes) node.UpdateUI();
        foreach (var node in enemyNodes) node.UpdateUI();
        UpdateTurnOrderDisplay();
        battleManager.ExecuteTurn();
    }
}
