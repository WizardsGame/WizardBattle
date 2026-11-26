# Класс `Wizard`

`Wizard` — общий класс для всех магов. Он хранит базовые характеристики и позицию на карте. Игрок-маг и будущие маги под управлением игры могут использовать общую логику этого класса.

## Поля и свойства

```csharp
[Export] public int speedTile = 5; // дальность перемещения за ход
[Export] public int health = 100;  // запас здоровья

public Vector2I CurPosition { get; } // текущая позиция в координатах клеток
```

`speedTile` и `health` экспортированы в Godot и могут настраиваться в инспекторе. `CurPosition` предназначена только для чтения: для перемещения используйте `PlaceOnCell()`.

## Метод

```csharp
public void PlaceOnCell(Vector2I targetCell, TileMapLayer groundLayer)
```

Перемещает мага на клетку `targetCell`: обновляет координату `CurPosition` и позицию узла Godot относительно слоя `groundLayer`. Передавайте слой `Ground` карты.
