using Godot;
using System;

public partial class Wizard : Node2D
{
	[Export] public int speedTile = 5;
	[Export] public int health = 100;
	[Export] public Label _nameLabel;
    public Vector2I curPosition;


    private bool _isMoving = true;
    
    public override void _Ready()
    {

    }

    public void PlaceOnCell(Vector2I cellPos, TileMapLayer groundLayer)
    {
        curPosition = cellPos;
        // MapToLocal переводит координаты тайлпапа в локальные координаты позиции node
        Position = groundLayer.MapToLocal(cellPos);
        GD.Print($"Position {curPosition.X}, {curPosition.Y}");
    }
}
