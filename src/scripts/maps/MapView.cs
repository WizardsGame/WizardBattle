using System;
using Godot;
using System.Collections.Generic;

namespace WizardBattle.Maps;

public partial class MapView : Node2D
{
	private readonly HashSet<Vector2I> _reachableHighlightCells = new();

	[Export]
	public TileMapLayer Ground { get; set; }

	[Export]
	public TileMapLayer Obstacles { get; set; }

	[Export]
    public TileMapLayer Highlight { get; set; }

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

	public void ShowHighlight(Dictionary<Vector2I, int> reachableCells)
    {
        if (Highlight == null) return;

	    _reachableHighlightCells.Clear();
        foreach (var pos in reachableCells.Keys)
	        _reachableHighlightCells.Add(pos);

	    RestoreReachableHighlights();
    }

	public void ShowPathPreview(IReadOnlyList<Vector2I> path, Vector2I startCell)
	{
		if (Highlight == null) return;

		RestoreReachableHighlights();
		for (int i = 0; i < path.Count; i++)
		{
			// На промежуточных клетках стрелка указывает на следующую клетку.
			// На последней — в сторону, откуда маг пришёл; для пути из одной клетки
			// источником направления служит стартовая позиция мага.
			Vector2I previousCell = i == 0 ? startCell : path[i - 1];
			Vector2I direction = i < path.Count - 1
				? path[i + 1] - path[i]
				: path[i] - previousCell;

			Vector2I arrowAtlasCoords = direction switch
			{
				{ X: 0, Y: -1 } => new Vector2I(0, 1), // вверх
				{ X: 0, Y: 1 } => new Vector2I(1, 1),  // вниз
				{ X: -1, Y: 0 } => new Vector2I(2, 1), // влево
				{ X: 1, Y: 0 } => new Vector2I(3, 1),  // вправо
				_ => new Vector2I(0, 0)
			};

			Highlight.SetCell(path[i], sourceId: 0, atlasCoords: arrowAtlasCoords);
		}
	}

	// При уходе курсора с доступной клетки восстанавливаем обычную подсветку диапазона.
	public void ClearPathPreview() => RestoreReachableHighlights();

	public void ClearSelectionHighlights()
	{
		_reachableHighlightCells.Clear();
		Highlight?.Clear();
	}

	private void RestoreReachableHighlights()
	{
		if (Highlight == null) return;

		Highlight.Clear();
		foreach (var position in _reachableHighlightCells)
			Highlight.SetCell(position, sourceId: 0, atlasCoords: new Vector2I(0, 0));
	}
}
