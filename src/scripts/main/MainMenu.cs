using Godot;

public partial class MainMenu : Control
{
	[Export] public string JoinLobbyScenePath = "res://src/scenes/lobby/lobby_join.tscn";
	[Export] public string CreateLobbyScenePath = "res://src/scenes/lobby/lobby.tscn";
	[Export] public string SettingsScenePath = "тут должен быть путь на настройки";

	public override void _Ready()
	{
	
		var joinLobbyButton = GetNode<Button>("%JoinLobbyButton");
		var createLobbyButton = GetNode<Button>("%CreateLobbyButton");
		var settingsButton = GetNode<Button>("%SettingsButton");

		
		joinLobbyButton.Pressed += OnJoinLobbyButtonPressed;
		createLobbyButton.Pressed += OnCreateLobbyButtonPressed;
		settingsButton.Pressed += OnSettingsButtonPressed;
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
