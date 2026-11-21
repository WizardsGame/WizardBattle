using Godot;
using System;
using System.Collections.Generic;
using WizardBattle.Lessons;
using WizardBattle.Session;

public partial class Lobby : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string ContinueScenePath = "на следующую сцену";

	private VBoxContainer _wordVBox;
	private LessonDraft _draft;

	private PanelContainer _modalBackdropRoot;
	private RichTextLabel _modalTitleLabel;
	private LineEdit _modalLineEdit;
	private Button _modalOkButton;

	private PackedScene _wordScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word.tscn");

	private Action<string> _currentOkEvent;

	public override void _Ready()
	{
		// подгружаем заранее
		_wordVBox = GetNode<VBoxContainer>("%WordsVBox");
		_modalBackdropRoot = GetNode<PanelContainer>("%ModalBackdropRoot");
		_modalTitleLabel = GetNode<RichTextLabel>("%ModalTitleLabel");
		_modalLineEdit = GetNode<LineEdit>("%ModalLineEdit");
		_modalOkButton = GetNode<Button>("%ModalOkButton");

		var wordAddButton = GetNode<Button>("%WordAddButton");
		var saveToFileButton = GetNode<Button>("%SaveToFileButton");
		var loadFromFileButton = GetNode<Button>("%LoadFromFileButton");
		var continueButton = GetNode<Button>("%ContinueButton");
		var backButton = GetNode<Button>("%BackButton");

		var modalCancelButton = GetNode<Button>("%ModalCancelButton");

		wordAddButton.Pressed += OnWordAddButtonPressed;
		saveToFileButton.Pressed += OnSaveToFileButtonPressed;
		loadFromFileButton.Pressed += OnLoadFromFileButtonPressed;
		continueButton.Pressed += OnContinueButtonPressed;
		backButton.Pressed += OnBackButtonPressed;

		_modalLineEdit.TextSubmitted += OnModalTextSubmitted;
		_modalOkButton.Pressed += OnModalConfirmed;
		modalCancelButton.Pressed += OnModalCancelButtonPressed;

		var session = GetNode<GameSession>("/root/GameSession");
		// Позволяет запускать лобби напрямую из редактора.
		if (session.Draft is null)
			session.BeginPreparation();

		_draft = session.Draft;
		RenderDraft();
	}

	private void OnWordAddButtonPressed()
	{
		OpenModal("Введите слово в начальной форме:", AddWord);
	}

	private void RenderDraft()
	{
		foreach (var child in _wordVBox.GetChildren())
		{
			_wordVBox.RemoveChild(child);
			child.QueueFree();
		}

		foreach (var entry in _draft.SentencesByWord)
			ShowWord(entry.Key, entry.Value);
	}

	private void AddWord(string text)
	{
		var sentences = new List<string>();
		if (!_draft.SentencesByWord.TryAdd(text, sentences))
		{
			GD.PrintErr($"Слово \"{text}\" уже добавлено.");
			return;
		}

		ShowWord(text, sentences);
	}

	private void ShowWord(string wordText, IEnumerable<string> sentences)
	{
		var wordView = _wordScene.Instantiate<LobbyWord>();
		wordView.SetWord(wordText);

		wordView.AddSentenceRequested += () =>
		{
			OpenModal("Введите предложение с #ответами#:\n[i]Важно: мест вставки для одного слова может быть несколько.\nНапример, для глагола to howl: \"#Does# the black moon #howl#?\"[/i]", sentence =>
			{
				AddSentence(wordText, wordView, sentence);
			});
		};
		wordView.RemoveRequested += () => RemoveWord(wordText, wordView);

		_wordVBox.AddChild(wordView);

		foreach (var sentence in sentences)
			ShowSentence(wordText, wordView, sentence);
	}

	private void AddSentence(string word, LobbyWord wordView, string sentenceText)
	{
		var sentences = _draft.SentencesByWord[word];
		if (sentences.Contains(sentenceText))
		{
			GD.PrintErr("Это предложение уже добавлено к слову.");
			return;
		}

		sentences.Add(sentenceText);
		ShowSentence(word, wordView, sentenceText);
	}

	private void ShowSentence(string word, LobbyWord wordView, string text)
	{
		var view = wordView.AddSentence(text);
		view.RemoveRequested += () => RemoveSentence(word, text, view);
	}

	private void RemoveWord(string word, LobbyWord view)
	{
		if (!_draft.SentencesByWord.Remove(word, out var sentences))
			return;

		foreach (var sentence in sentences)
			_draft.SpellIdBySentence.Remove(sentence);

		view.QueueFree();
	}

	private void RemoveSentence(string word, string text, LobbyWordSentence view)
	{
		if (!_draft.SentencesByWord.TryGetValue(word, out var sentences) ||
			!sentences.Remove(text))
		{
			return;
		}

		_draft.SpellIdBySentence.Remove(text);
		view.QueueFree();
	}

	private void OnSaveToFileButtonPressed()
	{
		//TODO: сохранение {слов:[]} в json файл
	}

	private void OnLoadFromFileButtonPressed()
	{
		//TODO: загрузка {слов:[]} из json файла
	}

	private void OnContinueButtonPressed()
	{
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
		if (ResourceLoader.Exists(CrossScenePath))
		{
			GetTree().ChangeSceneToFile(CrossScenePath);
		}
		else
		{
			GD.PrintErr($"Ошибка: Сцена по пути {CrossScenePath} не найдена!");
		}
	}

	private void OnModalCancelButtonPressed()
	{
		CloseModal();
	}

	private void OnModalTextSubmitted(string _)
	{
		OnModalConfirmed();
	}

	private void OnModalConfirmed()
	{
		var text = _modalLineEdit.Text.Trim();
		if (text.Length == 0)
			return;

		var action = _currentOkEvent;
		_currentOkEvent = null;

		action?.Invoke(text);
		CloseModal();
	}

	private void OpenModal(string titleText, Action<string> okEvent)
	{
		_modalTitleLabel.Text = titleText;
		_modalLineEdit.Clear();
		_currentOkEvent = okEvent;

		_modalBackdropRoot.Visible = true;
		_modalLineEdit.GrabFocus();
	}

	private void CloseModal()
	{
		_currentOkEvent = null;
		_modalLineEdit.Clear();
		_modalBackdropRoot.Visible = false;
	}
}
