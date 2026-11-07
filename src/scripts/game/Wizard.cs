using Godot;
using System;

namespace WizardBattle.Game;

public partial class Wizard : Node2D
{
	[Export] public int speedTile = 5;
	[Export] public int health = 100;
	private Label _nameLabel;
    private Vector2I _posInCells;

    //для set вызывайте PlaceOnCell
    public Vector2I CurPosition { 
        get => _posInCells;
    }

    public void PlaceOnCell(Vector2I targetCell, TileMapLayer groundLayer)
    {
        _posInCells = targetCell;
        // MapToLocal переводит координаты тайлпапа в локальные координаты позиции node
        Position = groundLayer.MapToLocal(targetCell);
        GD.Print($"Position {CurPosition.X}, {CurPosition.Y}");
    }
}
