using Godot;

namespace WizardBattle.Maps;

public partial class MapView : Node2D
{
	[Export]
	public TileMapLayer Ground { get; set; }

	[Export]
	public TileMapLayer Obstacles { get; set; }

	public override void _Ready()
	{
		RenderTestMap();
	}

	private void RenderTestMap()
	{
		Ground.Clear();
		Obstacles.Clear();

		const int width = 20;
		const int height = 12;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var position = new Vector2I(x, y);

				Ground.SetCell(
					position,
					sourceId: 0,
					atlasCoords: new Vector2I(0, 0)
				);

				bool isBorder =
					x == 0 || x == width - 1 ||
					y == 0 || y == height - 1;

				if (isBorder)
				{
					Obstacles.SetCell(
						position,
						sourceId: 0,
						atlasCoords: new Vector2I(0, 0)
					);
				}
			}
		}
	}
}
