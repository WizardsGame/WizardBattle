using System;
using System.Collections.Generic;
using Godot;
using WizardBattle.Maps;
using WizardBattle.Player;

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
            throw new InvalidOperationException("Обработчик нажатий не видит карту");
        }

        // клик мышкой
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            Vector2 globalMousePos = GetGlobalMousePosition();
            Vector2 localPos = World.Ground.ToLocal(globalMousePos);
            Vector2I clickedCell = World.Ground.LocalToMap(localPos);

            if (clickedCell.X < 0 || clickedCell.Y < 0 ||
                clickedCell.X >= CurrentMap.Width || clickedCell.Y >= CurrentMap.Height)
            {
                throw new InvalidOperationException("Нажатие вне поля");
            }

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

    private void HandleLeftMouseClick(Vector2I clickedCell)
    {
        switch (_inputMode)
        {
            case InputMode.Default:
                //Wizard clickedWizard = _wizards.Find(w => w.curPosition == clickedCell);
                if (clickedCell != _wizardPlayer.CurPosition) break;
                _selectedWizard = _wizardPlayer;
                _inputMode = InputMode.WizardPlayerSelected;
                var reachable = _wizardPlayer.CalculateReachableCells(CurrentMap, _wizards);
                World.ShowHighlight(reachable);
                GD.Print($"Выбран игрок {_selectedWizard.CurPosition}");
                break;

            case InputMode.WizardPlayerSelected:
                List<Vector2I> path = _wizardPlayer.GetPathTo(clickedCell);

                var reachableCells = _wizardPlayer.CalculateReachableCells(CurrentMap, _wizards);
                if (_wizardPlayer.CurPosition == clickedCell 
                || !reachableCells.ContainsKey(clickedCell)
                || _wizards.Exists(w => w.CurPosition == clickedCell))
                {
                    GD.Print("Выделение снято");
                    World.Highlight?.Clear();
                    _inputMode = InputMode.Default;
                    _selectedWizard = null;
                    break;
                }

                _selectedWizard.PlaceOnCell(clickedCell, World.Ground);
                foreach(var pos in reachableCells)
                {
                    GD.Print(pos);
                }
                GD.Print($"Построен путь длиной в {path.Count} шагов. Конечная цель: {clickedCell}");
                foreach(var pos in path)
                {
                    GD.Print($"({pos})->");
                }

                World.Highlight?.Clear();
                _inputMode = InputMode.Default;
                _selectedWizard = null;
                break;
        }
    }

    private void HandleRightMouseClick(Vector2I clickedCell)
    {
        VirtualCell cellData = CurrentMap.Cells[clickedCell.X, clickedCell.Y];
        _inputMode = InputMode.Default;
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
