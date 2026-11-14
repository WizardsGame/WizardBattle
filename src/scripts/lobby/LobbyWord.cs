using Godot;
using System;

public partial class LobbyWord : PanelContainer
{
	private VBoxContainer _sentencesVBox;
	
	private PackedScene _sentenceScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word_sentence.tscn");
	
	public override void _Ready()
	{
		//подгружаю заранее
		 _sentencesVBox = GetNode<VBoxContainer>("ItemMargin/ItemVBox/SentencesVBox");
		
		var WordRemoveButton = GetNode<Button>("ItemMargin/ItemVBox/HeaderHBox/WordRemoveButton");
		var AddSentenceButton = GetNode<Button>("ItemMargin/ItemVBox/AddSentenceButton");
		
		WordRemoveButton.Pressed += WordRemoveButtonPressed;
		AddSentenceButton.Pressed += AddSentenceButtonPressed;
	}
	
	private void WordRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить слово");
		QueueFree();
	}
	
	private void AddSentenceButtonPressed()
	{
		GD.Print("Нажата кнопка: Добавить предложение");
		
		//Создаем экземпляр сцены предложения
		Node sentenceInstance = _sentenceScene.Instantiate();
		Label sentenceLabel = sentenceInstance.GetNodeOrNull<Label>("SentenceMargin/SentenceHBox/SentenceLabel");
		
		if (sentenceLabel != null)
		{
			sentenceLabel.Text = "Тестовое предложение";
		}
		else
		{
			GD.PrintErr("Ошибка: Label для текста предложения не найден!");
		}
		_sentencesVBox.AddChild(sentenceInstance);
	}
}
