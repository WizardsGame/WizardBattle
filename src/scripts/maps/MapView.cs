using System;
using Godot;

namespace WizardBattle.Maps;

public partial class MapView : Node2D
{
	[Export]
	public TileMapLayer Ground { get; set; }

	[Export]
	public TileMapLayer Obstacles { get; set; }

	public void Render(MapData map)
	{
		ArgumentNullException.ThrowIfNull(map);
		if (Ground.TileSet is null || Obstacles.TileSet is null)
			throw new InvalidOperationException("У слоёв Ground и Obstacles должны быть назначены TileSet.");

		Ground.Clear();
		Obstacles.Clear();

		for (int y = 0; y < map.Height; y++)
		{
			for (int x = 0; x < map.Width; x++)
			{
				var position = new Vector2I(x, y);
				var cell = map.Cells[x, y];

				switch (cell)
				{
					case VoidCell:
						break;

					case WallCell:
						Ground.SetCell(position, sourceId: 0, atlasCoords: new Vector2I(0, 0));
						Obstacles.SetCell(position, sourceId: 0, atlasCoords: new Vector2I(0, 0));
						break;

					case FloorCell:
						Ground.SetCell(position, sourceId: 0, atlasCoords: new Vector2I(0, 0));
						break;

					default:
						throw new NotSupportedException($"Нет визуализации для клетки {cell.GetType().Name}.");
				}
			}
		}
	}
}
