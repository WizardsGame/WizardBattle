using Godot;
using System;

namespace WizardBattle.Game;

public partial class Wizard : Node2D
{
	[Export] public int speedTile = 5;
	[Export] public int health = 100;
	private Label _nameLabel;
    private Vector2I _posInCells;

    public Vector2I CurPosition { 
        get => _posInCells;
    }

    public Wizard()
    {
        _posInCells = new Vector2I(1, 1);
    }

    public Wizard(Vector2I startingPosInCells)
    {
        _posInCells = startingPosInCells;
    }

    public void PlaceOnCell(Vector2I targetCell, TileMapLayer groundLayer)
    {
        _posInCells = targetCell;
        // MapToLocal переводит координаты тайлпапа в локальные координаты позиции node
        Position = groundLayer.MapToLocal(targetCell);
        GD.Print($"Position {_posInCells.X}, {_posInCells.Y}");
    }
}
