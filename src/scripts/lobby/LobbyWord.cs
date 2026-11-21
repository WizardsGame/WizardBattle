using Godot;
using System;

public partial class LobbyWord : PanelContainer
{
	public event Action AddSentenceRequested;
	public event Action RemoveRequested;

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

	public void SetWord(string text)
	{
		var label = GetNode<Label>("%WordLabel");
		label.Text = text;
	}

	private void OnWordRemoveButtonPressed()
	{
		RemoveRequested?.Invoke();
	}

	public LobbyWordSentence AddSentence(string text)
	{
		var sentenceInstance = _sentenceScene.Instantiate<LobbyWordSentence>();
		sentenceInstance.SetSentence(text);
		_sentencesVBox.AddChild(sentenceInstance);
		return sentenceInstance;
	}

	private void OnAddSentenceButtonPressed()
	{
		AddSentenceRequested?.Invoke();
	}
}
