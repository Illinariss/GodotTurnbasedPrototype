using System.Collections.Generic;
using Godot;

public partial class Game : Control
{
    [Export] public PanelContainer MainContent;

    private Control previousContent;
    private BattleScene battleScene;

    public override void _Ready()
    {
        // // Handles only the first child
         previousContent = MainContent.GetChildCount() > 0 ? MainContent.GetChild<Control>(0) : null;

    }

    public void StartBattle(List<CharacterData> playerCharacters, List<CharacterData> enemies)
    {
        if (previousContent != null)
            previousContent.Visible = false;

        var scene = GD.Load<PackedScene>("res://battle_scene.tscn");
        battleScene = scene.Instantiate<BattleScene>();
        MainContent.AddChild(battleScene);
        // Optional: Charakterdaten an Szene übergeben
        battleScene.Setup(playerCharacters, enemies, OnBattleFinished);
    }

    public void OnBattleFinished()
    {
        battleScene.QueueFree();
        battleScene = null;

        if (previousContent != null)
            previousContent.Visible = true;

        // Weitere Logik nach Kampf
    }

    internal void _on_btn_kampf_1_pressed()
    {
        var playerImage = GD.Load<Texture2D>("res://assets/Example_Character_1.png");
        var bearImage = GD.Load<Texture2D>("res://assets/Bär in Bewegung.png");
        var wolfImage = GD.Load<Texture2D>("res://assets/Wolf in schwarzem Silhouettenprofil.png");

        var player = new CharacterData("Held", 100, 20, 15, 15, 15, playerImage);
        var bear = new CharacterData("Bär", 200, 0, 5, 8, 10, bearImage);
        var wolf = new CharacterData("Wolf", 50, 0, 20, 5, 5, wolfImage);

        StartBattle(new List<CharacterData> { player }, new List<CharacterData> { bear, wolf });

    }



}
