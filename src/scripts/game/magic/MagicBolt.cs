using Godot;

namespace WizardBattle.Game;

[GlobalClass]
public partial class MagicBolt : Magic
{
	// Значения зададаются в файле ресурса.
	
	public override void Cast(Wizard caster, Vector2I targetCell, Wizard targetWizard = null)
	{
		if (targetWizard == null)
		{
			GD.PrintErr($"{SpellName}: нужен target-игрок");
			return;
		}
		targetWizard.TakeDamage(Damage);
		GD.Print($"{caster.Name} бьёт {targetWizard.Name} на {Damage}");
	}
}
