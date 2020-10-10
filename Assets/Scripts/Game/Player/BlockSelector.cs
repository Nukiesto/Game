using LeopotamGroup.Math;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSelector : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private ChunkManager chunkManager;

    [Header("Другое")]
    [SerializeField] private BlockData blockToSet;
    [SerializeField, HideInInspector] private Vector3 onWorldPos;

    [SerializeField] private EventSystem sys;// = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    [SerializeField] private Inventory inventory;
    private string[] layers;
    //public void OnTriggerStay2D(Collider2D collision)
    //{
    //    canPlaceBlock = false;
    //}
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    canPlaceBlock = true;
    //}
    private void Start()
    {
        layers = new string[2];
        layers[0] = "Entity";
        layers[1] = "Item";
    }
    void Update()
    {
        onWorldPos = GetPoint();
        AlignPositionGrid(onWorldPos);

        ClickBlock();
    }
    
    private void ClickDeleteBlock()
    {
        ChunkUnit chunk = chunkManager.GetChunk(onWorldPos);
        //Debug.Log(chunk);
        chunk?.DeleteBlock(onWorldPos);
    }
    private void ClickPlaceBlock()
    {
        ItemUnit item = inventory.GetSelectedItem();
        if (item.data != null && item.data.type == ItemType.block)
        {
            if (!CheckCollisions())
            {
                ChunkUnit chunk = chunkManager.GetChunk(onWorldPos);

                if (chunk.SetBlock(onWorldPos, item.data.block, true))
                {
                    item.RemoveItem();
                }               
            }          
        }       
    }

    private bool CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        Vector3 pos = transform.position;
        pos.x -= 0.5f;
        pos.y -= 0.5f;
        List<RaycastHit2D[]> hits = new List<RaycastHit2D[]>();
        hits.Add(Physics2D.RaycastAll(pos, Vector2.right, 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x + 1, pos.y), Vector2.up, 1));
        hits.Add(Physics2D.RaycastAll(pos, Vector2.up, 1));
        hits.Add(Physics2D.RaycastAll(pos, new Vector2(0.5f, 0.5f), 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x + 1f, pos.y), new Vector2(-0.5f, 0.5f), 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x, pos.y + 0.1f), Vector2.right, 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x, pos.y + 1), Vector2.right, 1));

        for (int i = 0; i < hits.Count; i++)
        {
            for (int z = 0; z < hits[i].Length; z++)
            {
                if (hits[i][z].collider != null)
                {
                    string layer = LayerMask.LayerToName(hits[i][z].collider.gameObject.layer);
                    //Debug.Log(layer);
                    //Debug.Log(layer);
                    for (int j = 0; j < 2; j++)
                    {
                        //Debug.Log(layer + "; " + layers[j]);
                        if (layer == layers[j])
                        {
                            //Debug.Log("Has solid" + i);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    private void ClickBlock()
    {
        if (sys.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ClickDeleteBlock();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClickPlaceBlock();
        }       
    }
    
    //public void OnCollisionEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Enter");
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Cannot");
    //        canPlaceBlock = false;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    Debug.Log("Exit");
    //    //    if (collision.gameObject.CompareTag("Player"))
    //    //    {
    //    //        Debug.Log("Can");
    //    //        canPlaceBlock = true;
    //    //    }
    //}

    private void AlignPositionGrid(Vector3 pos)
    {
        pos.x = MathFast.Floor(pos.x) + 0.5f;
        pos.y = MathFast.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
    }

    private Vector3 GetPoint()
    {
        Vector3 pos = cameraMain.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        pos.z = 0;

        return pos;
    }
}
