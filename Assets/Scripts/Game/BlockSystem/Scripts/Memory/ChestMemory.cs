﻿using UnityEngine;

[CreateAssetMenu(menuName = "BlockMemories/Chest", fileName = nameof(ChestMemory))]
public class ChestMemory : BaseBlockMemory
{
    public int W => 8; 
    public int H => 4;
}

