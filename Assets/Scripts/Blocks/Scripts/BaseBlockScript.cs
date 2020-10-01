using UnityEngine;

public abstract class BaseBlockScript : ScriptableObject
{
    public virtual void UpdateBlock(BlockMemoryPackUpdaterBase packData = null) {}
}
