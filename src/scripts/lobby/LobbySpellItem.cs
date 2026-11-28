using System;
using System.Collections.Generic;
using Godot;

public partial class LobbySpellItem : PanelContainer
{
	public event Action<string> SpellSelected;

	public void Setup(string sentence, IReadOnlyList<(string Id, string Name)> spells, int selectedIndex)
	{
		GetNode<Label>("%SentenceLabel").Text = sentence;
		var options = GetNode<OptionButton>("%SpellOptions");
		foreach (var spell in spells)
			options.AddItem(spell.Name);

		options.Select(selectedIndex);
		options.ItemSelected += index => SpellSelected?.Invoke(spells[(int)index].Id);
	}
}
