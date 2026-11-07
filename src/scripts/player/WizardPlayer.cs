using Godot;
using System;
using System.Collections.Generic;
using WizardBattle.Game;
using WizardBattle.Maps;

namespace WizardBattle.Player;

// Маг управляемый игроком (singleton)
public partial class WizardPlayer : Wizard
{
    public static WizardPlayer Instance { get; private set; }
    private readonly Dictionary<Vector2I, Vector2I> _cameFrom = new();
    // Кэш последней рассчитанной зоны досягаемости
    private readonly Dictionary<Vector2I, int> _reachableCells = new();

    public override void _Ready()
    {
        base._Ready();

        if (Instance != null && Instance != this)
        {
            QueueFree(); // Защита от создания дубликатов
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Алгоритм Дейкстры для расчета доступных клеток с учетом SpeedCost и Weight.
    public Dictionary<Vector2I, int> CalculateReachableCells(MapData map, List<Wizard> otherWizards)
    {
        GD.Print($"дальность {speedTile}");
        _reachableCells.Clear();
        _cameFrom.Clear();

        var weights = new Dictionary<Vector2I, int>();
        var openSet = new List<(Vector2I Pos, int Cost)>();

        _reachableCells[CurPosition] = 0;
        weights[CurPosition] = map.Cells[CurPosition.X, CurPosition.Y].Weight;
        openSet.Add((CurPosition, 0));

        var directions = new Vector2I[]
        {
            new(0, 1), new(0, -1), new(1, 0), new(-1, 0)
        };

        while (openSet.Count > 0)
        {
            // Сортируем: сначала меньшая стоимость хода, при равенстве — больший вес
            openSet.Sort((a, b) =>
            {
                int costCompare = a.Cost.CompareTo(b.Cost);
                if (costCompare != 0) return costCompare;

                int weightA = weights.ContainsKey(a.Pos) ? weights[a.Pos] : 0;
                int weightB = weights.ContainsKey(b.Pos) ? weights[b.Pos] : 0;
                return weightB.CompareTo(weightA);
            });

            var current = openSet[0];
            openSet.RemoveAt(0);

            if (current.Cost > speedTile) continue;

            foreach (var dir in directions)
            {
                var neighborPos = current.Pos + dir;

                if (!map.IsWithinBoundary(neighborPos)) continue;

                var cell = map.Cells[neighborPos.X, neighborPos.Y];

                if (!cell.IsWalkable) continue;

                // Проверка, не занята ли клетка другими игроками
                if (otherWizards.Exists(w => w.CurPosition == neighborPos))
                    continue;

                int newCost = current.Cost + cell.SpeedCost;
                int newWeight = weights[current.Pos] + cell.Weight;

                if (newCost <= speedTile)
                {
                    bool isBetterPath = !_reachableCells.ContainsKey(neighborPos) || 
                                        newCost < _reachableCells[neighborPos] || 
                                        (newCost == _reachableCells[neighborPos] && newWeight > weights[neighborPos]);

                    if (isBetterPath)
                    {
                        _reachableCells[neighborPos] = newCost;
                        weights[neighborPos] = newWeight;
                        _cameFrom[neighborPos] = current.Pos; // Запоминаем шаг для восстановления пути
                        openSet.Add((neighborPos, newCost));
                    }
                }
            }
        }

        _reachableCells.Remove(CurPosition); // Исключаем стартовую клетку
        return _reachableCells;
    }


    // Восстанавливает и возвращает список клеток пути от текущей позиции до целевой.
    public List<Vector2I> GetPathTo(Vector2I targetCell)
    {
        var path = new List<Vector2I>();
        if (!_reachableCells.ContainsKey(targetCell)) return path;

        var current = targetCell;
        while (current != CurPosition)
        {
            path.Add(current);
            if (_cameFrom.TryGetValue(current, out var next))
            {
                current = next;
            }
            else
            {
                break;
            }
        }
        path.Reverse(); // Переворачиваем, чтобы путь шел от старта к цели
        return path;
    }
}
