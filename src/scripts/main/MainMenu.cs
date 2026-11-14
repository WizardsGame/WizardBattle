using Godot;

public partial class MainMenu : Control
{
	[Export] public string JoinLobbyScenePath = "res://src/scenes/lobby/lobby_connecting.tscn";
	[Export] public string CreateLobbyScenePath = "res://src/scenes/lobby/lobby.tscn";
	[Export] public string SettingsScenePath = "тут должен быть путь на настройки";

	public override void _Ready()
	{
	
		var JoinLobbyButton = GetNode<Button>("VBoxContainer/HBoxContainer/JoinLobbyButton");
		var CreateLobbyButton = GetNode<Button>("VBoxContainer/HBoxContainer/CreateLobbyButton");
		var SettingsButton = GetNode<Button>("VBoxContainer/SettingsButton");

		
		JoinLobbyButton.Pressed += OnJoinLobbyButtonPressed;
		CreateLobbyButton.Pressed += OnCreateLobbyButtonPressed;
		SettingsButton.Pressed += OnSettingsButtonPressed;
	}

	private void OnJoinLobbyButtonPressed()
	{
		GD.Print("Нажата кнопка: Присоединиться к комнате");
		
		if (ResourceLoader.Exists(JoinLobbyScenePath))
		{
			GetTree().ChangeSceneToFile(JoinLobbyScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {JoinLobbyScenePath} не найдена!");
		}
	}

	private void OnCreateLobbyButtonPressed()
	{
		GD.Print("Нажата кнопка: Создать комнату");
		
		if (ResourceLoader.Exists(CreateLobbyScenePath))
		{
			GetTree().ChangeSceneToFile(CreateLobbyScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {CreateLobbyScenePath} не найдена!");
		}
	}
	
	private void OnSettingsButtonPressed()
	{
		//TODO: Создать сцену настроек и реализовать переход на неё здесь
		GD.Print("Нажата кнопка: Настройки");
		
		if (ResourceLoader.Exists(SettingsScenePath))
		{
			GetTree().ChangeSceneToFile(SettingsScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {SettingsScenePath} не найдена!");
		}
	}
}
