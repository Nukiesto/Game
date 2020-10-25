﻿using System.Collections.Generic;
 using UnityEngine;

[CreateAssetMenu(menuName = "BlockMemories/Chest", fileName = nameof(ChestMemory))]
public class ChestMemory : BaseBlockMemory
{
    public List<ItemData.Data> items;
}

