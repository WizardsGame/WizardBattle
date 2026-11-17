using Godot;
using System;

public partial class Lobby : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/main/main.tscn";
	[Export] public string ContinueScenePath = "на следующую сцену";

	private VBoxContainer _wordVBox;

	private PanelContainer _modalBackdropRoot;
	private Label _modalTitleLabel;
	private LineEdit _modalLineEdit;
	private Button _modalOkButton;

	private PackedScene _wordScene = GD.Load<PackedScene>("res://src/scenes/lobby/lobby_word.tscn");

	private Action _currentOkEvent;

	public override void _Ready()
	{
		// подгружаем заранее
		_wordVBox = GetNode<VBoxContainer>("%WordsVBox");
		_modalBackdropRoot = GetNode<PanelContainer>("%ModalBackdropRoot");
		_modalTitleLabel = GetNode<Label>("%ModalTitleLabel");
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
	}

	private void OnWordAddButtonPressed()
	{
		GD.Print("Нажата кнопка: Добавить слово");

		OpenModal("Введите слово в начальной форме:", () =>
		{
			var word = _modalLineEdit.Text;

			// TODO: добавление слова в некоторую реальную структуру с данными!

			Node wordInstance = _wordScene.Instantiate();
			Label wordLabel = wordInstance.GetNodeOrNull<Label>("ItemMargin/ItemVBox/HeaderHBox/WordLabel");
			if (wordLabel == null)
			{
				GD.PrintErr("Ошибка: Label для слова предложения не найден!");
				return;
			}

			wordLabel.Text = word;
			_wordVBox.AddChild(wordInstance);
		});

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

	private void OnModalCancelButtonPressed()
	{
		GD.Print("Нажата кнопка: Отмена в модальном окне ввода");

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

		_modalLineEdit.Text = text;
		
		var action = _currentOkEvent;
		_currentOkEvent = null;

		action?.Invoke();
		CloseModal();
	}

	private void OpenModal(string titleText, Action okEvent)
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
