using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/BlocksDataBase", fileName = "BlockDataBase")]
public class BlockDataBase : ScriptableObject
{
    private const string dirPath = "Assets/Resources/Blocks/";
    private Dictionary<string, BlockData> blocks;

    public void OnEnable()
    {
        InitBlocks();
    }
    public void InitBlocks()
    {
        Debug.Log("Refreshed");
        var folders = Directory.GetDirectories(dirPath);
        BlockData block;
        string path;
        blocks = new Dictionary<string, BlockData>();
        for (var i = 0; i < folders.Length; i++)
        {
            //folders[i].Replace(V, "/");
            //Debug.Log(folders[i]);
            path = folders[i].Remove(0, 17) + "/" + folders[i].Remove(0, 24);
            block = Resources.Load<BlockData>(path);
            //Debug.Log(path);
            if (block != null)
            {
                //Debug.Log(block.nameBlock);
                blocks.Add(block.nameBlock, block);              
            }
        }
    }
    public BlockData GetBlock(string name)
    {  
        return blocks[name];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BlockDataBase))]
public class LabelsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        BlockDataBase dataBase = (BlockDataBase)target;
        EditorUtility.SetDirty(target);

        if (GUILayout.Button("Refresh"))
        {
            dataBase.InitBlocks();
        }
    }
}
#endif
