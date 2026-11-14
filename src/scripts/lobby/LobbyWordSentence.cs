using Godot;
using System;

public partial class LobbyWordSentence : PanelContainer
{
	public override void _Ready()
	{
		var sentenceRemoveButton = GetNode<Button>("%SentenceRemoveButton");
		
		sentenceRemoveButton.Pressed += OnSentenceRemoveButtonPressed;
	}
	
	private void OnSentenceRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить предложение");
		
		QueueFree();
	}
}
