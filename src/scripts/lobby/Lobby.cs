using Godot;
using System;

public partial class Lobby : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string ContinueScenePath = "на следующую сцену";
	
	private VBoxContainer _wordVBox;
	
	private PackedScene _wordScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word.tscn");
	
	public override void _Ready()
	{
		//подгружаю заранее
		 _wordVBox = GetNode<VBoxContainer>("%WordsVBox");
		
		var wordAddButton = GetNode<Button>("%WordAddButton");
		var saveToFileButton = GetNode<Button>("%SaveToFileButton");
		var loadFromFileButton = GetNode<Button>("%LoadFromFileButton");
		var continueButton = GetNode<Button>("%ContinueButton");
		var backButton = GetNode<Button>("%BackButton");
		
		wordAddButton.Pressed += OnWordAddButtonPressed;
		saveToFileButton.Pressed += OnSaveToFileButtonPressed;
		loadFromFileButton.Pressed += OnLoadFromFileButtonPressed;
		continueButton.Pressed += OnContinueButtonPressed;
		backButton.Pressed += OnBackButtonPressed;
	}

	private void OnWordAddButtonPressed()
	{
		GD.Print("Нажата кнопка: Добавить слово");
		
		//Создаем экземпляр сцены предложения
		Node wordInstance = _wordScene.Instantiate();
		Label wordLabel = wordInstance.GetNodeOrNull<Label>("ItemMargin/ItemVBox/HeaderHBox/WordLabel");
		
		if (wordLabel != null)
		{
			wordLabel.Text = "Тестовое слово";
		}
		else
		{
			GD.PrintErr("Ошибка: Label для слова предложения не найден!");
		}
		_wordVBox.AddChild(wordInstance);
	}
	
	private void OnSaveToFileButtonPressed()
	{
		GD.Print("Нажата кнопка: Сохранить в файл");
		
		//TODO: сохранение {слов:[]} в json файл
	}

	private void OnLoadFromFileButtonPressed()
	{
		GD.Print("Нажата кнопка: Загрузить из файла");
		
		//TODO: загрузка {слов:[]} из json файла
	}

	private void OnContinueButtonPressed()
	{
		GD.Print("Нажата кнопка: Далее");
		
		if (ResourceLoader.Exists(ContinueScenePath))
		{
			GetTree().ChangeSceneToFile(ContinueScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {ContinueScenePath} не найдена!");
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
