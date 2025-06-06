using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CharacterNode : Node2D
{
    [Export] public Texture2D CharacterImage;
    private Sprite2D _sprite;
    private bool _isHovered = false;
    private bool _isCurrentFighter = false;
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;
    private Label _nameLabel;

    public CharacterData Data;
    private ShaderMaterial _shadermaterial;


    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("%CharacterTexture");        
        _hpBar = GetNode<ProgressBar>("%HPBar");
        _manaBar = GetNode<ProgressBar>("%ManaBar");
        _nameLabel = GetNode<Label>("%NameLabel");
        if (CharacterImage != null)
            _sprite.Texture = CharacterImage;
        _shadermaterial = new ShaderMaterial();
        var shader = GD.Load<Shader>("uid://bfpky86adx6j8");
        _shadermaterial.Shader = shader;
        _sprite.Material = _shadermaterial;


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
            GD.Print("_greenshader");
            
            _shadermaterial.SetShaderParameter("line_color", new Color(0, 1, 0, 1));
            _shadermaterial.SetShaderParameter("line_thickness", 10.0f);
            return;
        }
        if (_isHovered)
        {
            if (this.Data.IsPlayerCharacter)
            {
                GD.Print("_whiteshader");
                _shadermaterial.SetShaderParameter("line_color", new Color(1, 1, 1, 1));
                _shadermaterial.SetShaderParameter("line_thickness", 10.0f);
                
            }
            else
            {
                GD.Print("_redshader");
                _shadermaterial.SetShaderParameter("line_color", new Color(1, 0, 0, 1));
                _shadermaterial.SetShaderParameter("line_thickness", 10.0f);                
            }
        }
        else
        {
            _shadermaterial.SetShaderParameter("line_color", new Color(0, 0, 0, 0));
        }
        
    }
    public void SetData(CharacterData data)
    {
        Data = data;
        data.Node = this;
        if (data.CharacterImage != null)
            CharacterImage = data.CharacterImage;        
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
