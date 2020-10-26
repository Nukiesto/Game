﻿using UnityEngine;
 
 [System.Serializable]
[CreateAssetMenu(menuName = "BlockMemories/Dirt", fileName = nameof(DirtMemory))]
public class DirtMemory : BaseBlockMemory
{ 
 public override MemoryUnit memoryUnit { get; set; }
}

