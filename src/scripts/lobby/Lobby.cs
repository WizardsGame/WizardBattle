using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WizardBattle.Lessons;
using WizardBattle.Session;

public partial class Lobby : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string ContinueScenePath = "на следующую сцену";

	private VBoxContainer _wordVBox;
	private LessonDraft _draft;
	private FileDialog _saveFileDialog;
	private FileDialog _loadFileDialog;
	private AcceptDialog _fileErrorDialog;
	private ConfirmationDialog _importDialog;
	private LessonDraft _pendingImport;

	private PanelContainer _modalBackdropRoot;
	private RichTextLabel _modalTitleLabel;
	private Label _modalErrorLabel;
	private LineEdit _modalLineEdit;
	private Button _modalOkButton;

	private PackedScene _wordScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word.tscn");

	private Action<string> _currentOkEvent;
	private Func<string, string> _currentValidator;

	public override void _Ready()
	{
		// подгружаем заранее
		_wordVBox = GetNode<VBoxContainer>("%WordsVBox");
		_modalBackdropRoot = GetNode<PanelContainer>("%ModalBackdropRoot");
		_modalTitleLabel = GetNode<RichTextLabel>("%ModalTitleLabel");
		_modalErrorLabel = GetNode<Label>("%ModalErrorLabel");
		_modalLineEdit = GetNode<LineEdit>("%ModalLineEdit");
		_modalOkButton = GetNode<Button>("%ModalOkButton");
		_saveFileDialog = GetNode<FileDialog>("%SaveFileDialog");
		_loadFileDialog = GetNode<FileDialog>("%LoadFileDialog");
		_fileErrorDialog = GetNode<AcceptDialog>("%FileErrorDialog");
		_importDialog = GetNode<ConfirmationDialog>("%ImportDialog");
		_importDialog.AddButton("Заменить", false, "replace");
		_importDialog.Confirmed += () => CompleteImport(0);
		_importDialog.CustomAction += action =>
		{
			if (action == "replace") CompleteImport(1);
		};
		_importDialog.Canceled += () => CompleteImport(2);
		_saveFileDialog.FileSelected += SaveLesson;
		_loadFileDialog.FileSelected += LoadLesson;

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
			}, ValidateSentenceMarkup);
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

	private static string ValidateSentenceMarkup(string text)
	{
		SentenceMarkup.TryValidate(text, out var error);
		return error;
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
		_saveFileDialog.PopupFileDialog();
	}

	private void OnLoadFromFileButtonPressed()
	{
		_loadFileDialog.PopupFileDialog();
	}

	private void SaveLesson(string path)
	{
		try
		{
			LessonDraftFile.Save(path, _draft);
		}
		catch (Exception error) when (IsFileError(error))
		{
			ShowFileError("Не удалось сохранить материал", error);
		}
	}

	private void LoadLesson(string path)
	{
		try
		{
			// Читаем и проверяем полностью, прежде чем заменять текущий материал.
			var newDraft = LessonDraftFile.Load(path);
			if (newDraft.SentencesByWord.Count > 0 && _draft.SentencesByWord.Count > 0)
			{
				_pendingImport = newDraft;
				ShowImportChoice();
			}
			else
			{
				ApplyDraft(newDraft);
			}
		}
		catch (Exception error) when (IsFileError(error))
		{
			ShowFileError("Не удалось загрузить материал", error);
		}
	}

	private void ShowImportChoice()
	{
		if (DisplayServer.HasFeature(DisplayServer.Feature.NativeDialog))
		{
			var result = DisplayServer.DialogShow(_importDialog.Title, _importDialog.DialogText,
				new[] { "Добавить", "Заменить", "Отмена" }, Callable.From<int>(CompleteImport));
			if (result == Error.Ok) return;
		}
		// На платформах без системных текстовых диалогов используем окно Godot.
		_importDialog.PopupCentered();
	}

	private void CompleteImport(int choice)
	{
		var imported = _pendingImport;
		_pendingImport = null;
		_importDialog.Hide();
		if (imported is null || !IsInsideTree()) return;
		try
		{
			if (choice == 0) ApplyDraft(MergeDraft(imported));
			else if (choice == 1) ApplyDraft(imported);
		}
		catch (InvalidDataException error)
		{
			ShowFileError("Не удалось объединить материал", error);
		}
	}

	private LessonDraft MergeDraft(LessonDraft imported)
	{
		var merged = new LessonDraft();
		var owners = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var entry in _draft.SentencesByWord)
		{
			merged.SentencesByWord.Add(entry.Key, new List<string>(entry.Value));
			foreach (var sentence in entry.Value) owners[sentence] = entry.Key;
		}
		foreach (var entry in imported.SentencesByWord)
		{
			if (!merged.SentencesByWord.TryGetValue(entry.Key, out var sentences))
			{
				sentences = new List<string>();
				merged.SentencesByWord.Add(entry.Key, sentences);
			}
			foreach (var sentence in entry.Value)
			{
				if (owners.TryGetValue(sentence, out var owner) && owner != entry.Key)
					throw new InvalidDataException("Одно предложение связано с разными словами.");
				if (!sentences.Contains(sentence)) sentences.Add(sentence);
				owners[sentence] = entry.Key;
			}
		}
		merged.SpellIdBySentence = new Dictionary<string, string>(_draft.SpellIdBySentence, StringComparer.Ordinal);
		return merged;
	}

	private void ApplyDraft(LessonDraft draft)
	{
		GetNode<GameSession>("/root/GameSession").ReplaceDraft(draft);
		_draft = draft;
		RenderDraft();
	}

	private static bool IsFileError(Exception error) =>
		error is IOException or InvalidDataException or UnauthorizedAccessException or JsonException
			or ArgumentException or NotSupportedException;

	private void ShowFileError(string title, Exception error)
	{
		GD.PrintErr($"{title}: {error.Message}");
		_fileErrorDialog.Title = title;
		_fileErrorDialog.DialogText = error switch
		{
			JsonException => "Это не файл учебного материала. Выберите JSON, сохранённый в лобби.",
			InvalidDataException when title == "Не удалось объединить материал" =>
				"Одно предложение связано с разными словами. Данные не изменены.",
			InvalidDataException => "В файле некорректные слова или предложения. Проверьте данные и пары #ответ#.",
			UnauthorizedAccessException => "Нет доступа к файлу. Выберите другую папку или проверьте права доступа.",
			FileNotFoundException or DirectoryNotFoundException => "Файл или папка не найдены. Выберите файл заново.",
			IOException => "Не удалось прочитать или записать файл. Проверьте доступность диска и свободное место.",
			_ => "Не удалось открыть выбранный путь. Выберите другой файл или папку."
		};
		_fileErrorDialog.PopupCentered();
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
		{
			ShowModalError("Введите текст.");
			return;
		}

		var error = _currentValidator?.Invoke(text);
		if (error != null)
		{
			ShowModalError(error);
			return;
		}

		var action = _currentOkEvent;
		_currentOkEvent = null;
		_currentValidator = null;

		action?.Invoke(text);
		CloseModal();
	}

	private void OpenModal(
		string titleText,
		Action<string> okEvent,
		Func<string, string> validator = null)
	{
		_modalTitleLabel.Text = titleText;
		_modalLineEdit.Clear();
		_currentOkEvent = okEvent;
		_currentValidator = validator;
		_modalErrorLabel.Text = string.Empty;
		_modalErrorLabel.Visible = false;

		_modalBackdropRoot.Visible = true;
		_modalLineEdit.GrabFocus();
	}

	private void ShowModalError(string error)
	{
		_modalErrorLabel.Text = $"Ошибка: {error}";
		_modalErrorLabel.Visible = true;
		_modalLineEdit.GrabFocus();
	}

	private void CloseModal()
	{
		_currentOkEvent = null;
		_currentValidator = null;
		_modalErrorLabel.Text = string.Empty;
		_modalErrorLabel.Visible = false;
		_modalLineEdit.Clear();
		_modalBackdropRoot.Visible = false;
	}
}
