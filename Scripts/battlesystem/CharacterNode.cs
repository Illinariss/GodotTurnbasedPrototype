using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CharacterNode : Node2D
{
    [Export] public Texture2D CharacterImage;
    private Sprite2D _sprite;
    private ShaderMaterial _shader;
    private bool _isHovered = false;
    private bool _isCurrentFighter = false;
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;

    public CharacterData Data;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("%CharacterSprite");
        _shader = _sprite.Material as ShaderMaterial;
        _hpBar = GetNode<ProgressBar>("%HPBar");
        _manaBar = GetNode<ProgressBar>("%ManaBar");

        if (CharacterImage != null)
            _sprite.Texture = CharacterImage;

        _sprite.Connect("mouse_entered", new Callable(this, nameof(OnMouseEntered)));
        _sprite.Connect("mouse_exited", new Callable(this, nameof(OnMouseExited)));

        UpdateUI();
        UpdateOutline();
    }

    private void OnMouseEntered()
    {
        _isHovered = true;
        UpdateOutline();
    }

    private void OnMouseExited()
    {
        _isHovered = false;
        UpdateOutline();
    }

    public void SetCurrentFighter(bool value)
    {
        _isCurrentFighter = value;
        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (_shader == null) return;

        if (_isCurrentFighter)
        {
            _shader.SetShaderParameter("line_color", new Color(0, 1, 0, 1));
            _shader.SetShaderParameter("line_thickness", 10.0f);
            return;
        }

        if (_isHovered)
        {
            _shader.SetShaderParameter("line_color", new Color(1, 1, 1, 1));
            _shader.SetShaderParameter("line_thickness", 10.0f);
        }
        else
        {
            _shader.SetShaderParameter("line_thickness", 0.0f);
        }
    }
    public void SetData(CharacterData data)
    {
        Data = data;
        data.Node = this;
        if (data.CharacterImage != null)
            CharacterImage = data.CharacterImage;
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
        var scene = GD.Load<PackedScene>("res://Scripts/battlesystem/CharacterNode.tscn");
        var node = scene.Instantiate<CharacterNode>();
        node.SetData(data);
        return node;
    }

}
