using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CharacterNode : Node2D
{
    [Export] public Texture2D CharacterImage;
    private TextureRect _image;
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;

    public CharacterData Data;

    public override void _Ready()
    {
        _image = GetNode<TextureRect>("%CharacterTexture");
        _hpBar = GetNode<ProgressBar>("%HPBar");
        _manaBar = GetNode<ProgressBar>("%ManaBar");

        if (CharacterImage != null)
            _image.Texture = CharacterImage;

        UpdateUI();
    }

    public void SetData(CharacterData data)
    {
        Data = data;
        data.Node = this;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (Data == null) return;
        _hpBar.MaxValue = Data.MaxHP;
        _hpBar.Value = Data.CurrentHP;
        _manaBar.MaxValue = Data.MaxMana;
        _manaBar.Value = Data.CurrentMana;
    }

    public static CharacterNode Create(CharacterData data)
    {
        var scene = GD.Load<PackedScene>("res://scenes/ui/CharacterNode.tscn");
        var node = scene.Instantiate<CharacterNode>();
        node.SetData(data);
        return node;
    }

}
