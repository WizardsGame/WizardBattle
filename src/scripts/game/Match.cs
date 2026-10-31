using System;
using Godot;
using WizardBattle.Maps;

namespace WizardBattle.Game;

public partial class Match : Node2D
{
    [Export]
    public MapView World { get; set; }

    [Export]
    public string[] ArenaPaths { get; set; } = new[]
    {
        "res://src/maps/arena_open.json",
        "res://src/maps/arena_corridors.json",
        "res://src/maps/arena_divided.json"
    };

    // -1 - случайная арена
    [Export]
    public int ArenaIndex { get; set; } = -1;

    public MapData CurrentMap { get; private set; }

    public override void _Ready()
    {
        try
        {
            if (ArenaPaths is null || ArenaPaths.Length == 0)
                throw new InvalidOperationException("Список ArenaPaths пуст.");

            if (ArenaIndex < -1 || ArenaIndex >= ArenaPaths.Length)
                throw new InvalidOperationException("ArenaIndex должен быть -1 или индексом из ArenaPaths.");

            int index = ArenaIndex == -1 ? GD.RandRange(0, ArenaPaths.Length - 1) : ArenaIndex;
            CurrentMap = new MapLoader().Load(ArenaPaths[index]);
            World.Render(CurrentMap);

            GD.Print($"Арена: {CurrentMap.Id} ({CurrentMap.Width} x {CurrentMap.Height}), индекс {index}.");
        }
        catch (Exception exception)
        {
            GD.PushError($"Не удалось запустить арену: {exception}");
        }
    }
}
