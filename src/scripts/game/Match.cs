using System;
using System.Collections.Generic;
using Godot;
using WizardBattle.Maps;

namespace WizardBattle.Game;

public enum InputMode
{
    Default, // Ожидание выбора
    WizardPlayerSelected // Игрок выбрал себя, ждём клетку куда переместиться
}

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

    private InputMode _inputMode = InputMode.Default;

    // -1 - случайная арена
    [Export]
    public int ArenaIndex { get; set; } = -1;

    public MapData CurrentMap { get; private set; }

    //------пока я спавню только одного мага, прямо в match--------
    //TODO: надо будет сделать отдельный спавнер 
    private Wizard _selectedWizard;
    private List<Wizard> _wizards = new();

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

            LoadWizards();

        }
        catch (Exception exception)
        {
            GD.PushError($"Не удалось запустить арену: {exception}");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (CurrentMap == null || World?.Ground == null)
        {
            throw new InvalidOperationException("Обработчик нажатий не видит карту");
        }

        // клик мышкой
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            Vector2 globalMousePos = GetGlobalMousePosition();
            Vector2 localPos = World.Ground.ToLocal(globalMousePos);
            Vector2I clickedCell = World.Ground.LocalToMap(localPos);
            GD.Print($"Клик по клетке: X = {clickedCell.X}, Y = {clickedCell.Y}");

            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                HandleLeftMouseClick(clickedCell);
            }

            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                HandleRightMouseClick(clickedCell);
            }
        }
    }

    private void HandleLeftMouseClick(Vector2I clickedCell)
    {
        switch (_inputMode)
        {
            case InputMode.Default:
                Wizard clickedWizard = _wizards.Find(w => w.curPosition == clickedCell);
                if(clickedWizard == null) break;
                _selectedWizard = clickedWizard;
                _inputMode = InputMode.WizardPlayerSelected;
                GD.Print($"Выбран игрок {_selectedWizard.curPosition}");
                break;

            case InputMode.WizardPlayerSelected:
                _selectedWizard.PlaceOnCell(clickedCell, World.Ground);
                _selectedWizard = null;
                _inputMode = InputMode.Default;
                break;
        }
    }

    private void HandleRightMouseClick(Vector2I clickedCell)
    {
        VirtualCell cellData = CurrentMap.Cells[clickedCell.X, clickedCell.Y];
        GD.Print($"Клетка {cellData.GetType().Name}");
    }

    // Собираем всех магов из узла World/Wizards
    private void LoadWizards()
    {
        Node2D wizardsParent = GetNodeOrNull<Node2D>("World/Wizards");
        if (wizardsParent == null) return;

        _wizards.Clear();

        for (int i = 0; i < wizardsParent.GetChildCount(); i++)
        {
            if (wizardsParent.GetChild(i) is Wizard curWizard)
            {
                _wizards.Add(curWizard);
            }
        }
    }
}
