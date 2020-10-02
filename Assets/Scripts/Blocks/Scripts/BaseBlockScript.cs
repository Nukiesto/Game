using UnityEngine;

public abstract class BaseBlockScript : ScriptableObject
{
    public virtual void UpdateBlock(BlockMemoryPackToUpdaterBase packData = null) {}
}
