using Godot;
using System;

namespace WizardBattle.Game;

public partial class Wizard : Node2D
{
	[Export] public int speedTile { get; set; } = 5;
	[Export] public int Health { get; set; } = 100;
	[Export] public int MaxHealth { get; set; } = 100;
	
	[Export] public Godot.Collections.Array<Magic> Spellbook { get; set; } = new();
	
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
	
	public void TakeDamage(int amount)
	{
		Health = Mathf.Max(0, Health - amount);
		GD.Print($"{Name} получает {amount} урона. HP: {Health}/{MaxHealth}");
	}

	// Единая точка входа для каста — проверка маны + полиморфный вызов
	public bool TryCast(Magic magic, Vector2I targetCell, Wizard targetWizard = null)
	{
		// Нужно условие проверики что введёное слово верно
		magic.Cast(this, targetCell, targetWizard);
		return true;
	}
}
