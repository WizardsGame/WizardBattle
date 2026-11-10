using Godot;
using System;

public partial class LobbyCreating : Control
{
	
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string NextScenePath = "res://src/scenes/lobby/lobby.tscn";
	
	public override void _Ready()
	{
		var SaveButton = GetNode<Button>("VBoxContainer/HBoxContainer/SaveButton");
		var NextButton = GetNode<Button>("VBoxContainer/HBoxContainer/NextButton");
		var CrossButton = GetNode<Button>("CrossButton");
		
		SaveButton.Pressed += OnSaveButtonPressed;
		NextButton.Pressed += OnNextButtonPressed;
		CrossButton.Pressed += OnCrossButtonPressed;
	}

	private void OnSaveButtonPressed()
	{
		//ToDo: сохранить и другие настройки
	}
	
	private void OnNextButtonPressed()
	{
		GD.Print("Нажата кнопка: Переход на следущую сцену");
		
		if (ResourceLoader.Exists(NextScenePath))
		{
			GetTree().ChangeSceneToFile(NextScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {NextScenePath} не найдена!");
		}
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
