using System.Collections.Generic;

namespace WizardBattle.Maps;

public abstract class VirtualCell
{
    public virtual int SpeedCost => 1;

    public virtual int Weight => 100;

    public virtual bool IsWalkable => true;

    public List<VirtualTileEffect> TileEffects { get; } = new();
}
