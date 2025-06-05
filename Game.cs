using System.Collections.Generic;
using Godot;

public partial class Game : Control
{
    [Export] public PanelContainer MainContent;

    private Control previousContent;
    private BattleScene battleScene;

    public override void _Ready()
    {
        //regelt nur ein Child
        previousContent = MainContent.GetChildCount() > 0 ? MainContent.GetChild<Control>(0) : null;

    }

    public void StartBattle(List<CharacterData> playerCharacters, List<CharacterData> enemies)
    {
        if (previousContent != null)
            previousContent.Visible = false;

        var scene = GD.Load<PackedScene>("res://scenes/battle/BattleScene.tscn");
        battleScene = scene.Instantiate<BattleScene>();
        // Optional: Charakterdaten an Szene übergeben
        battleScene.Setup(playerCharacters, enemies, OnBattleFinished);

        MainContent.AddChild(battleScene);
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




    }



}
