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
		 _wordVBox = GetNode<VBoxContainer>("RootPanel/RootMargin/MenuPanel/MenuVBox/WordsMargin/WordsScroll/WordsScrollMargin/WordsVBox");
		
		var WordAddButton = GetNode<Button>("RootPanel/RootMargin/MenuPanel/MenuVBox/WordAddButtonMargin/WordAddButton");
		var SaveToFileButton = GetNode<Button>("RootPanel/RootMargin/MenuPanel/MenuVBox/ButtonsMargin/ButtonsHBox/SaveToFileButton");
		var ContinueButton = GetNode<Button>("RootPanel/RootMargin/MenuPanel/MenuVBox/ButtonsMargin/ButtonsHBox/ContinueButton");
		var CrossButton = GetNode<Button>("CrossButton");
		
		WordAddButton.Pressed += WordAddButtonPressed;
		SaveToFileButton.Pressed += SaveToFileButtonPressed;
		ContinueButton.Pressed += ContinueButtonPressed;
		CrossButton.Pressed += OnCrossButtonPressed;
	}

	private void WordAddButtonPressed()
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
	
	private void SaveToFileButtonPressed()
	{
		GD.Print("Нажата кнопка: Сохранить в файл");
		
		//TODO: сохранение {слов:[]} в json файл
	}

	private void ContinueButtonPressed()
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
