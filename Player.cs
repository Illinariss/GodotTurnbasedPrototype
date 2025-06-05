using Godot;
using System;

public partial class Player : Node2D
{
     [Export] public Texture2D CharacterImage;
    private TextureRect _image;
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;

    public CharacterData Data;

    public override void _Ready()
    {
        _image = GetNode<TextureRect>("CharacterTexture");
        _hpBar = GetNode<ProgressBar>("HPBar");
        _manaBar = GetNode<ProgressBar>("ManaBar");

        if (CharacterImage != null)
            _image.Texture = CharacterImage;

        UpdateBars();
    }

    public void SetData(CharacterData data)
    {
        Data = data;
        UpdateBars();
    }

    public void UpdateBars()
    {
        if (Data == null) return;

        _hpBar.MaxValue = Data.MaxHP;
        _hpBar.Value = Data.CurrentHP;

        _manaBar.MaxValue = Data.MaxMana;
        _manaBar.Value = Data.CurrentMana;
    }
}
