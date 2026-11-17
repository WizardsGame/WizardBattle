using Godot;
using System;

public partial class LobbyWordSentence : PanelContainer
{
	public override void _Ready()
	{
		var sentenceRemoveButton = GetNode<Button>("%SentenceRemoveButton");

		sentenceRemoveButton.Pressed += OnSentenceRemoveButtonPressed;
	}

	public void SetSentence(string text)
	{
		var label = GetNode<Label>("%SentenceLabel");
		label.Text = text;
	}

	private void OnSentenceRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить предложение");

		// TODO: самоудаление из структуры с данными. вероятно, делегирование классу `Lobby`

		QueueFree();
	}
}
