using Godot;

namespace WizardBattle.Game;

[GlobalClass]
public partial class Teleport : Magic
{
	// Значения зададаются в файле ресурса.
	
	[Export] public int MaxDistance { get; set; } = 3;

	public override void Cast(Wizard caster, Vector2I targetCell, Wizard targetWizard = null)
	{
		var distance = Mathf.Abs(targetCell.X - caster.CurPosition.X)
					 + Mathf.Abs(targetCell.Y - caster.CurPosition.Y); // манхэттенское расстояние
		if (distance > MaxDistance)
		{
			GD.PrintErr($"{SpellName}: слишком далеко ({distance} > {MaxDistance})");
			return;
		}
		// caster.PlaceOnCell(targetCell, groundLayer) — нужен доступ к слою
		GD.Print($"{caster.Name} телепортируется в {targetCell}");
	}
}
