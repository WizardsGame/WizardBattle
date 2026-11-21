using Godot;

namespace WizardBattle.Game;

[GlobalClass]
public partial class Fireball : Magic
{
	// Значения зададаются в файле ресурса.

	public override void Cast(Wizard caster, Vector2I targetCell, Wizard targetWizard = null)
	{
		GD.Print($"{caster.Name} кастует {SpellName} в клетку {targetCell}");
	}
}
