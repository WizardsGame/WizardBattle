using Godot;

namespace WizardBattle.Game;

[GlobalClass]
public partial class Heal : Magic
{
	[Export] public int HealAmount { get; set; } = 30;
	// Значения зададаются в файле ресурса.

	public override void Cast(Wizard caster, Vector2I targetCell, Wizard targetWizard = null)
	{
		if (targetWizard == null)
		{
			GD.PrintErr($"{SpellName}: нужен-игрок для лечения");
			return;
		}
		targetWizard.Health = Mathf.Min(targetWizard.Health + HealAmount, targetWizard.MaxHealth);
		GD.Print($"{caster.Name} лечит {targetWizard.Name} на {HealAmount}");
	}
}
