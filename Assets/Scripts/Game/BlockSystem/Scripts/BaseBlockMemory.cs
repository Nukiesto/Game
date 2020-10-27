using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class BaseBlockMemory : ScriptableObject
{
    public virtual BaseBlockMemory GetMemoryUnit()
    {
        return null;
    }

    public virtual void SetMemoryUnit(string memory)
    {
        
    }
}
