using System;
using Singleton;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CircleCollider2D itemMagnet;
    [SerializeField] private float itemPickRadius;
    [SerializeField] private GameObject itemCreatePos;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private GameObject flashLight;

    private bool _flashLightActive;
    public static PlayerController Instance;
    
    private void Start()
    {
        Instance = this;
        itemMagnet.radius = itemPickRadius;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _flashLightActive = !_flashLightActive;
            flashLight.SetActive(_flashLightActive);
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name);
        if (col.gameObject.CompareTag("Item"))
        {
            var obj = col.gameObject;
            //Debug.Log(obj.GetComponent<Item>().data);
            inventory.AddItem(obj.GetComponent<Item>().data);
            obj.SetActive(false);
        }
    }
    public bool CanToCreateItem()
    {
        var pos = itemCreatePos.transform.position;
        return !chunkManager.GetChunk(pos).HasBlock(pos);
    }
    public void CreateItemKick(ItemData.Data data, int count)
    {
        var pos = itemCreatePos.transform.position;

        for (var i = 0; i < count; i++)
        {
            Toolbox.Instance.mItemManager.CreateItem(pos, data);
        }       
    }
}
