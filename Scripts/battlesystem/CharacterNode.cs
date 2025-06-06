using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CharacterNode : Node2D
{
    [Export] public Texture2D CharacterImage;
    private Sprite2D _sprite;
    private ShaderMaterial _greenshader;
    private ShaderMaterial _whiteshader;
    private ShaderMaterial _redshader;

    private bool _isHovered = false;
    private bool _isCurrentFighter = false;
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;
    private Label _nameLabel;

    public CharacterData Data;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("%CharacterTexture");
        _greenshader = GD.Load<ShaderMaterial>("uid://c6pt7umnbysyt");
        _whiteshader = GD.Load<ShaderMaterial>("uid://38mg6q7dh3vl");
        _redshader = GD.Load<ShaderMaterial>("uid://b3k6vlw6bpvl6");
        _hpBar = GetNode<ProgressBar>("%HPBar");
        _manaBar = GetNode<ProgressBar>("%ManaBar");
        _nameLabel = GetNode<Label>("%NameLabel");

        if (CharacterImage != null)
            _sprite.Texture = CharacterImage;

        // this.Connect("mouse_entered", new Callable(this, nameof(OnMouseEntered)));
        // this.Connect("mouse_exited", new Callable(this, nameof(OnMouseExited)));

        UpdateUI();
        UpdateOutline();
    }

    private void _on_character_body_2d_mouse_entered()
    {
        GD.Print("OnMouseEntered");
        _isHovered = true;
        UpdateOutline();
    }

    private void _on_character_body_2d_mouse_exited()
    {
        GD.Print("OnMouseExited");
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
        if (_sprite == null) return;

        if (_isCurrentFighter)
        {
            _sprite.Material = _greenshader;
            // _whiteshader.SetShaderParameter("line_color", new Color(0, 1, 0, 1));
            // _whiteshader.SetShaderParameter("line_thickness", 10.0f);
            return;
        }

        if (_isHovered)
        {
            if (this.Data.IsPlayerCharacter)
            {
                _sprite.Material = _whiteshader;
            }
            else
            {
                _sprite.Material = _redshader;
            }            
        }
        else
        {
            _whiteshader.SetShaderParameter("line_thickness", 0.0f);
        }
    }
    public void SetData(CharacterData data)
    {
        Data = data;
        data.Node = this;
        if (data.CharacterImage != null)
            CharacterImage = data.CharacterImage;

        // UpdateUI will be called in _Ready() once the node is fully
        // initialized. Avoid calling it here when UI nodes are not yet
        // available.
        if (_hpBar != null && _manaBar != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (Data == null) return;
        _hpBar.MaxValue = Data.MaxHP;
        _hpBar.Value = Data.CurrentHP;
        _manaBar.MaxValue = Data.MaxMana;
        _manaBar.Value = Data.CurrentMana;
        if (_nameLabel != null)
            _nameLabel.Text = Data.Name;
    }

    public static CharacterNode Create(CharacterData data)
    {
        var scene = GD.Load<PackedScene>("res://Scripts/battlesystem/CharacterNode.tscn");
        var node = scene.Instantiate<CharacterNode>();
        node.SetData(data);
        return node;
    }

}
