using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [SerializeField, HideInInspector] private BlockData[] data;
    [SerializeField, HideInInspector] private Dictionary<BlockData, Texture2D> texture;
    private void Awake()
    {
        data = Resources.LoadAll<BlockData>("Data/Blocks");
        texture = new Dictionary<BlockData, Texture2D>();

        //for (int i = 0; i < data.Length; i++)
        //{
        //    Debug.Log("i:" + i + " ;" + data[i].name);
        //}
    }
}
