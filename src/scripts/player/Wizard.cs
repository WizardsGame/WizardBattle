using Godot;
using System;

public partial class Wizard : Node2D
{
	public NodePath tileMapPath = "/root/src/scenes/game/Match/World/Ground";
	[Export] public int speedTile = 5;
	[Export] public int health = 100;
	private TileMapLayer _tileMap;
	private Label _label;
    private Vector2I _curPosition;
	private Sprite2D _sprite;
    private bool _isMoving = true;

    //private Vector2 _moveTarget;
    
    
    
    // [Signal]
    // public delegate void WizardSelectedEventHandler();
    
    // [Signal]
    // public delegate void MoveCompleteEventHandler();
    
    public override void _Ready()
    {
        // Получаем ссылки на дочерние ноды
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _label = GetNode<Label>("Label");
        
        // Получаем TileMap из структуры сцены
        //_tileMap = GetNode<TileMapLayer>(tileMapPath);
        
        // Инициализируем позицию
        //_curPosition = _tileMap.LocalToMap(GlobalPosition);
        
        // Устанавливаем обработчик входа для Sprite2D
        // _sprite.GuiInput += OnSpriteGuiInput;
        // _sprite.MouseFilter = Control.MouseFilterEnum.Stop;
    }

	
}
