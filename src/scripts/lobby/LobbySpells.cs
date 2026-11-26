using System;
using System.Collections.Generic;
using Godot;
using WizardBattle.Session;

public partial class LobbySpells : CanvasLayer
{
	[Export] public string CrossScenePath = "res://src/scenes/lobby/lobby.tscn";
	[Export] public string ContinueScenePath = "res://src/scenes/game/match.tscn";
	[Export] public PackedScene SpellItemScene { get; set; }

	// TODO: убрать мок, составлять пары по объектам заклинаний из общего реестра реально существующих заклинаний
	private readonly List<(string Id, string Name)> _availableSpells = new()
	{
		("fireball", "Огненный шар"),
		("ice_arrow", "Ледяная стрела"),
		("shield", "Щит"),
	};
	private readonly Dictionary<string, string> _selectedSpells = new(StringComparer.Ordinal);
	private GameSession _session;
	private Button _continueButton;
	private int _sentenceCount;

	public override void _Ready()
	{
		_session = GetNode<GameSession>("/root/GameSession");
		_continueButton = GetNode<Button>("%ContinueButton");
		_continueButton.Pressed += OnContinueButtonPressed;
		GetNode<Button>("%BackButton").Pressed += OnBackButtonPressed;

		var items = GetNode<VBoxContainer>("%SpellItemsVBox");
		var draft = _session.Draft;
		if (draft != null)
		{
			foreach (var sentencesList in draft.SentencesByWord.Values)
			{
				foreach (var sentence in sentencesList)
				{
					int selectedIndex = -1;
					if (draft.SpellIdBySentence.TryGetValue(sentence, out var spellId))
					{
						selectedIndex = _availableSpells.FindIndex(spell => spell.Id == spellId);
						if (selectedIndex >= 0)
							_selectedSpells.Add(sentence, spellId);
					}

					var item = SpellItemScene.Instantiate<LobbySpellItem>();
					item.Setup(sentence, _availableSpells, selectedIndex);
					item.SpellSelected += selectedId => OnSpellSelected(sentence, selectedId);
					items.AddChild(item);
					_sentenceCount++;
				}
			}
		}

		UpdateContinueButton();
	}

	private void OnSpellSelected(string sentence, string spellId)
	{
		_selectedSpells[sentence] = spellId;
		UpdateContinueButton();
	}

	private void UpdateContinueButton()
	{
		_continueButton.Disabled = _sentenceCount == 0 || _selectedSpells.Count != _sentenceCount;
	}

	private void OnBackButtonPressed()
	{
		if (ChangeScene(CrossScenePath))
			SaveAssignments();
	}

	private void OnContinueButtonPressed()
	{
		if (_continueButton.Disabled || !ChangeScene(ContinueScenePath)) return;

		SaveAssignments();
		_session.StartMatch();
	}

	private void SaveAssignments()
	{
		if (_session.Draft is { } draft)
			draft.SpellIdBySentence = new Dictionary<string, string>(_selectedSpells, StringComparer.Ordinal);
	}

	private bool ChangeScene(string path)
	{
		var error = GetTree().ChangeSceneToFile(path);
		if (error == Error.Ok) return true;

		GD.PushError($"Не удалось открыть сцену {path}: {error}");
		return false;
	}
}
