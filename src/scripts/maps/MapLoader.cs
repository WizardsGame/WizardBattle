using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WizardBattle.Maps;

public class MapLoader
{
    public MapData Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        if (file is null)
            throw new IOException($"Не удалось открыть карту '{path}': {Godot.FileAccess.GetOpenError()}.");

        try
        {
            return Parse(file.GetAsText());
        }
        catch (Exception exception) when (
            exception is JsonException or FormatException or ArgumentException)
        {
            throw new FormatException($"Некорректная карта '{path}': {exception.Message}", exception);
        }
    }

    internal static MapData Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
            throw new FormatException("Корень JSON должен быть объектом.");

        string id = ReadString(root, "id");

        if (!root.TryGetProperty("rows", out var rowsElement) ||
            rowsElement.ValueKind != JsonValueKind.Array || rowsElement.GetArrayLength() == 0)
        {
            throw new FormatException("rows должен быть непустым массивом строк.");
        }

        var rows = new List<string>();
        foreach (var row in rowsElement.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.String)
                throw new FormatException("Каждый элемент rows должен быть строкой.");

            rows.Add(row.GetString());
        }

        int width = rows[0].Length;
        if (width == 0)
            throw new FormatException("Строки карты не могут быть пустыми.");

        var cells = new VirtualCell[width, rows.Count];
        for (int y = 0; y < rows.Count; y++)
        {
            if (rows[y].Length != width)
                throw new FormatException($"Строка {y}: все строки карты должны иметь одинаковую длину.");

            for (int x = 0; x < width; x++)
            {
                cells[x, y] = rows[y][x] switch
                {
                    '.' => new FloorCell(),
                    '#' => new WallCell(),
                    ' ' => new VoidCell(),
                    var symbol => throw new FormatException($"Неизвестный символ '{symbol}' в клетке ({x}, {y}).")
                };
            }
        }

        return new MapData(id, cells);
    }

    private static string ReadString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) ||
            value.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(value.GetString()))
        {
            throw new FormatException($"'{name}' должен быть непустой строкой.");
        }

        return value.GetString();
    }
}
