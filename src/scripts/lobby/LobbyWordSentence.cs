using Godot;
using System;

public partial class LobbyWordSentence : PanelContainer
{
	public override void _Ready()
	{
		var SentenceRemoveButton = GetNode<Button>("SentenceMargin/SentenceHBox/SentenceRemoveButton");
		
		SentenceRemoveButton.Pressed += SentenceRemoveButtonPressed;
	}
	
	private void SentenceRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить предложение");
		
		QueueFree();
	}
}
