using Godot;
using System;

public partial class LobbyWord : PanelContainer
{
	private VBoxContainer _sentencesVBox;
	
	private PackedScene _sentenceScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word_sentence.tscn");
	
	public override void _Ready()
	{
		//подгружаю заранее
		 _sentencesVBox = GetNode<VBoxContainer>("%SentencesVBox");
		
		var wordRemoveButton = GetNode<Button>("%WordRemoveButton");
		var addSentenceButton = GetNode<Button>("%AddSentenceButton");
		
		wordRemoveButton.Pressed += OnWordRemoveButtonPressed;
		addSentenceButton.Pressed += OnAddSentenceButtonPressed;
	}
	
	private void OnWordRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить слово");
		QueueFree();
	}
	
	private void OnAddSentenceButtonPressed()
	{
		GD.Print("Нажата кнопка: Добавить предложение");
		
		Node sentenceInstance = _sentenceScene.Instantiate();
		Label sentenceLabel = sentenceInstance.GetNodeOrNull<Label>("%SentenceLabel");
		
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
