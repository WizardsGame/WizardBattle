using Godot;
using System;

public partial class LobbyJoin : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string JoinLobbyScenePath = "тут должен быть путь к сцене лобби";
	
	public override void _Ready()
	{
		var joinLobbyButton = GetNode<Button>("%JoinLobbyButton");
		var backButton = GetNode<Button>("%BackButton");
		
		joinLobbyButton.Pressed += OnJoinLobbyButtonPressed;
		backButton.Pressed += OnBackButtonPressed;
	}
	
	private void OnJoinLobbyButtonPressed()
	{
		GD.Print("Нажата кнопка: Присоединение к комнате");
	
		if (ResourceLoader.Exists(JoinLobbyScenePath))
		{
			GetTree().ChangeSceneToFile(JoinLobbyScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {JoinLobbyScenePath} не найдена!");
		}
	}
	
	private void OnBackButtonPressed()
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
