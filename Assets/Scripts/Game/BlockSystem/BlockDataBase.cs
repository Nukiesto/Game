using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/BlocksDataBase", fileName = "BlockDataBase")]
public class BlockDataBase : ScriptableObject
{
    public Blocks blocksData;
    [Serializable] 
    public class Blocks
    {
        public BlockData[] blocks;
    }

    public BlockData GetBlock(string name)
    {
        for (int i = 0; i < blocksData.blocks.Length; i++)
        {
            if (blocksData.blocks[i].nameBlock == name)
            {
                return blocksData.blocks[i];
            }
        }
        return null;
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(BlockDataBase))]
//public class LabelsDataEditor : Editor
//{
//	public override void OnInspectorGUI()
//	{
//		DrawDefaultInspector();

//		BlockDataBase dataBase = (BlockDataBase)target;
//		EditorUtility.SetDirty(target);
//	}
//}
//#endif
