using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CircleCollider2D itemMagnet;
    [SerializeField] private float itemPickRadius;
    [SerializeField] private GameObject itemCreatePos;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ChunkManager chunkManager;
    void Start()
    {
        itemMagnet.radius = itemPickRadius;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name);
        if (col.gameObject.name == "Item(Clone)")
        {
            GameObject obj = col.gameObject;
            //Debug.Log(obj.GetComponent<Item>().data);
            inventory.AddItem(obj.GetComponent<Item>().data);
            obj.SetActive(false);
        }
    }
    public bool CanToCreateItem()
    {
        Vector3 pos = itemCreatePos.transform.position;
        return !chunkManager.GetChunk(pos).HasBlock(pos);
    }
    public void CreateItemKick(ItemData.Data data, int count)
    {
        Vector3 pos = itemCreatePos.transform.position;

        for (int i = 0; i < count; i++)
        {
            ItemManager.CreateItem(pos, data);
        }       
    }
}
