using Godot;

public partial class MainMenu : Control
{

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
		//TODO: Переход на сцену Lobby
        GD.Print("Нажата кнопка: Присоединиться к комнате");
    }

	private void OnCreateLobbyButtonPressed()
    {
		//TODO: Создать сцену создания лобби и реализовать переход на неё здесь
        GD.Print("Нажата кнопка: Создать комнату");
    }
	private void OnSettingsButtonPressed()
    {
		//TODO: Создать сцену настроек и реализовать переход на неё здесь
        GD.Print("Нажата кнопка: Настройки");
    }
}
