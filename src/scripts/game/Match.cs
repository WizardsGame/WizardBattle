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

    //пока я спавню только одного мага, прямо в match
    //в будущем надо будет сделать отдельный спавнер 
    private Wizard _wizard;

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


            // -------------Временное решение--------------

            _wizard = GetNodeOrNull<Wizard>("World/Wizards/Wizard");

            // Располагаем мага на клетке (0, 0)
            _wizard.PlaceOnCell(new Vector2I(0, 0), World.Ground);

            // -------------Временное решение---------------
            
        }
        catch (Exception exception)
        {
            GD.PushError($"Не удалось запустить арену: {exception}");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // клик левой кнопкой мыши
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (World?.Ground != null)
            {
                Vector2 globalMousePos = GetGlobalMousePosition();
                Vector2 localPos = World.Ground.ToLocal(globalMousePos);
                Vector2I clickedCell = World.Ground.LocalToMap(localPos);
                
                GD.Print($"Клик по клетке: X = {clickedCell.X}, Y = {clickedCell.Y}");
                _wizard.PlaceOnCell(new Vector2I(clickedCell.X, clickedCell.Y), World.Ground);
            }
        }
    }
}
