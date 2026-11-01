using System;

namespace WizardBattle.Maps;

public class MapData
{
    public string Id { get; }

    public VirtualCell[,] Cells { get; }

    public int Width => Cells.GetLength(0);
    public int Height => Cells.GetLength(1);

    public MapData(string id, VirtualCell[,] cells)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(cells);

        if (cells.GetLength(0) == 0 || cells.GetLength(1) == 0)
            throw new ArgumentException("Карта не может быть пустой.", nameof(cells));

        foreach (var cell in cells)
        {
            if (cell is null)
                throw new ArgumentException("Все клетки карты должны быть заполнены.", nameof(cells));
        }

        Id = id;
        Cells = cells;
    }
}
