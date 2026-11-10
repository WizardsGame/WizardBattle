using Godot;
using System;

public partial class LobbyConnecting : Control
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string ConnectingLobbyScenePath = "тут должен быть путь к сцене лобби";
	
	public override void _Ready()
	{
		var ConnectingLobbyButton = GetNode<Button>("VBoxContainer/ConnectingLobbyButton");
		var CrossButton = GetNode<Button>("CrossButton");
		
		ConnectingLobbyButton.Pressed += OnConnectingLobbyButtonPressed;
		CrossButton.Pressed += OnCrossButtonPressed;
	}
	
	private void OnConnectingLobbyButtonPressed()
	{
		GD.Print("Нажата кнопка: Присоединение к комнате");
	
		if (ResourceLoader.Exists(ConnectingLobbyScenePath))
		{
			GetTree().ChangeSceneToFile(ConnectingLobbyScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {ConnectingLobbyScenePath} не найдена!");
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
