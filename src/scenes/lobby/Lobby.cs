using Godot;
using System;

public partial class Lobby : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	
	
	public override void _Ready()
	{
		var CrossButton = GetNode<Button>("CrossButton");
		
		CrossButton.Pressed += OnCrossButtonPressed;
	}

	private void OnCrossButtonPressed()
	{
		GD.Print("Нажата кнопка: Крестик");
		
		if (ResourceLoader.Exists(CrossScenePath))
		{
			GetTree().ChangeSceneToFile(CrossScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {CrossScenePath} не найдена!");
		}
	}
}
