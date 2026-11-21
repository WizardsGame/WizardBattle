// Magic.cs
using Godot;
using System;

namespace WizardBattle.Game;

[GlobalClass]
public abstract partial class Magic : Resource
{
	[Export] public string SpellName { get; set; } = "Заклинание";
	[Export] public int Damage { get; set; } = 0;

	// Единый контракт: каст по клетке или по игроку.
	// Абстрактный метод — наследники обязаны реализовать.
	public abstract void Cast(Wizard caster, Vector2I targetCell, Wizard targetWizard = null);
}
