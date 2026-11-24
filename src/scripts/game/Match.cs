using System;
using System.Collections.Generic;
using Godot;
using WizardBattle.Maps;
using WizardBattle.Player;

namespace WizardBattle.Game;

public enum InputMode
{
    Default, // Ожидание выбора
    WizardPlayerSelected, // Игрок выбрал себя, ждём клетку куда переместиться
    CastingSpell // Выбрано заклинание, ждём цель заклинания
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

    private WizardPlayer _wizardPlayer; // сам игрок (singleton)
    private List<Wizard> _wizards = new(); // другие игроки
    private WizardPlayer _selectedWizard;

    //-------------------------------------------------------------

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
            return;
        }

        // Пока выбран маг, обновляем маршрут при наведении на клетку.
        if (@event is InputEventMouseMotion && _inputMode == InputMode.WizardPlayerSelected)
        {
            if (TryGetCellUnderMouse(out var hoveredCell))
                UpdatePathPreview(hoveredCell);
            else
                World.ClearPathPreview();

            return;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (!TryGetCellUnderMouse(out var clickedCell)) return;

            GD.Print($"Клик по клетке: X = {clickedCell.X}, Y = {clickedCell.Y}");

            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                HandleLeftMouseClick(clickedCell);
            }

            else if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                HandleRightMouseClick(clickedCell);
            }
        }
    }

    private bool TryGetCellUnderMouse(out Vector2I cell)
    {
        Vector2 localPos = World.Ground.ToLocal(GetGlobalMousePosition());
        cell = World.Ground.LocalToMap(localPos);

        return cell.X >= 0 && cell.Y >= 0 &&
               cell.X < CurrentMap.Width && cell.Y < CurrentMap.Height;
    }

    private void HandleLeftMouseClick(Vector2I clickedCell)
    {
        switch (_inputMode)
        {
            case InputMode.Default:
                //Wizard clickedWizard = _wizards.Find(w => w.curPosition == clickedCell);
                if (_wizardPlayer == null || clickedCell != _wizardPlayer.CurPosition) break;
                BeginWizardSelection(_wizardPlayer);
                break;

            case InputMode.WizardPlayerSelected:
                if (_selectedWizard == null || clickedCell == _selectedWizard.CurPosition ||
                    !_wizardPlayer.IsCellReachable(clickedCell) ||
                    _wizards.Exists(w => w.CurPosition == clickedCell))
                {
                    GD.Print("Выделение снято");
                    ChangeInputMode(InputMode.Default);
                    break;
                }

                // Используем тот же расчёт достижимости, который был сделан при выборе мага.
                // GetPathTo восстанавливает путь из этого кэша, не запуская новый поиск.
                List<Vector2I> path = _wizardPlayer.GetPathTo(clickedCell);
                _selectedWizard.PlaceOnCell(clickedCell, World.Ground);
                GD.Print($"Построен путь длиной в {path.Count} шагов. Конечная цель: {clickedCell}");
                foreach (var pos in path)
                {
                    GD.Print($"({pos})->");
                }

                ChangeInputMode(InputMode.Default);
                break;

            case InputMode.CastingSpell:
                // TODO: Обработка цели заклинания
                break;
        }
    }

    private void BeginWizardSelection(WizardPlayer wizard)
    {
        _selectedWizard = wizard;
        ChangeInputMode(InputMode.WizardPlayerSelected);

        var reachableCells = wizard.CalculateReachableCells(CurrentMap, _wizards);
        World.ShowHighlight(reachableCells);
        GD.Print($"Выбран игрок {_selectedWizard.CurPosition}");
    }

    // Точка входа для UI/системы заклинаний: после выбора заклинания можно вызвать этот метод.
    public void BeginCastingSpell()
    {
        if (_wizardPlayer == null) return;

        _selectedWizard = _wizardPlayer;
        ChangeInputMode(InputMode.CastingSpell);
    }

    private void UpdatePathPreview(Vector2I targetCell)
    {
        if (_wizardPlayer == null || !_wizardPlayer.IsCellReachable(targetCell) ||
            _wizards.Exists(w => w.CurPosition == targetCell))
        {
            World.ClearPathPreview();
            return;
        }

        World.ShowPathPreview(_wizardPlayer.GetPathTo(targetCell), _wizardPlayer.CurPosition);
    }

    private void ChangeInputMode(InputMode nextMode)
    {
        if (_inputMode == nextMode) return;

        if (_inputMode == InputMode.WizardPlayerSelected)
            World?.ClearSelectionHighlights();

        _inputMode = nextMode;
        if (nextMode == InputMode.Default)
            _selectedWizard = null;
    }

    private void HandleRightMouseClick(Vector2I clickedCell)
    {
        VirtualCell cellData = CurrentMap.Cells[clickedCell.X, clickedCell.Y];
        ChangeInputMode(InputMode.Default);
        GD.Print($"Клетка {cellData.GetType().Name}");
    }

    //----------------------------Временно--------------------------------------------
    // Сейчас мы собираем всех ЗАРАНЕЕ вручную созданных магов из узла World/Wizards
    // TODO: Сделать спавнер, который будет создавать одного WizardPlayer и много Wizard
    private void LoadWizards()
    {
        if (WizardPlayer.Instance == null)
        {
            GD.PrintErr("На сцене не найден WizardPlayer! Убедитесь, что узел добавлен");
            return;
        }
        // Локального игрока берем напрямую через синглтон
        _wizardPlayer = WizardPlayer.Instance;
        _wizardPlayer.PlaceOnCell(new Vector2I(1, 1), World.Ground);

        // В цикле собираем ТОЛЬКО остальных игроков
        var wizardsParent = GetNodeOrNull<Node>("World/Wizards");
        if (wizardsParent == null) return;

        _wizards.Clear();
        int childCount = wizardsParent.GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            var child = wizardsParent.GetChild(i);

            // Добавляем ноду, если это маг, но НЕ наш локальный игрок
            if (child is Wizard wizard && wizard != _wizardPlayer)
            {
                _wizards.Add(wizard);
            }
        }
    }
}
