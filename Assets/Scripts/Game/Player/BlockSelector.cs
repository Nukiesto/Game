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

    [SerializeField] private EventSystem sys;
    [SerializeField] private Inventory inventory;

    //Destroying Block
    [Header("Уничтожение блока")]
    [SerializeField] private Sprite[] destroyingStadies;
    [SerializeField] private SpriteRenderer destroyingProcess;

    private bool isDeleting;
    private Vector3 posBlockDelete;
    private float currentTime;
    private float blockToDeleteTime;
    private float procent;//
    private ChunkUnit chunkUnitClick;//Чанк на который нажали
    [SerializeField] private float powerDig;//Мощность всапывания

    private float asd;

    private List<string> layers;

    //public void OnTriggerStay2D(Collider2D collision)
    //{
    //    canPlaceBlock = false;
    //}
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    canPlaceBlock = true;
    //}
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

    private void Start()
    {
        layers = new List<string>();
        layers.Add("Entity");
        layers.Add("Item");
    }
    private void Update()
    {
        if (!isDeleting)
        {
            onWorldPos = GetPoint();
            AlignPositionGrid(onWorldPos);
        }
        else
        {
            DeletingUpdate();
            if (!InBounds(onWorldPos) || Input.GetMouseButtonUp(0))
            {
                DeletingReset();
                //Debug.Log("Deleting Abort");
            }
        }
        ClickBlock();
    }

    #region Deleting Time
    private void DeletingUpdate()
    {
        currentTime += Time.deltaTime;

        SetSprite();
        if (currentTime > blockToDeleteTime)
        {
            DeletingBlock();
        }
    }
    private void SetSprite()
    {
        procent = currentTime / blockToDeleteTime * 100;
        //Debug.Log(procent);
        float m = 0;
        for (int i = 0; i < 10; i++)
        {
            m += 10f;//12.5f;
            if (procent <= m)
            {
                destroyingProcess.sprite = destroyingStadies[i];
                return;
            }        
        }
    }
    private void DeletingReset()
    {
        currentTime = 0;
        blockToDeleteTime = 0;
        
        isDeleting = false;
        procent = 0;

        destroyingProcess.sprite = destroyingStadies[0];
        destroyingProcess.enabled = false;
    }
    private void DeletingBlock()
    {
        chunkUnitClick.DeleteBlock(posBlockDelete);    
        DeletingReset();
    }
    #endregion

    private void ClickPlaceBlock()
    {
        ItemUnit item = inventory.GetSelectedItem();
        if (item.data != null && item.data.type == ItemType.block)
        {
            if (!CheckCollisions())
            {
                if (chunkManager.GetChunk(onWorldPos).SetBlock(onWorldPos, item.data.block, true))
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
                    //return layers.Contains(layer);
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
        if (sys.IsPointerOverGameObject())//Если на UI
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {           
            if (!isDeleting)
            {
                chunkUnitClick = chunkManager.GetChunk(onWorldPos);

                posBlockDelete = onWorldPos;

                float hp = chunkUnitClick?.
                                    GetBlockUnit(onWorldPos, BlockLayer.front)?
                                    .data.hp ?? 0;
                
                if (hp != 0)
                {
                    if (hp < 0)
                    {
                        DeletingBlock();
                    }
                    else
                    {
                        blockToDeleteTime = hp / powerDig;
                        destroyingProcess.enabled = true;
                        isDeleting = true;
                        //Debug.Log("StartDeleting");
                    }                   
                }                                              
            }
        }
           
                             
        
        

        if (Input.GetMouseButtonDown(1))
        {
            ClickPlaceBlock();
        }       
    }    

    private void AlignPositionGrid(Vector3 pos)
    {
        pos.x = MathFast.Floor(pos.x) + 0.5f;
        pos.y = MathFast.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
    }
    private bool InBounds(Vector3 pos)
    {       
        pos.x = MathFast.Floor(pos.x);
        pos.y = MathFast.Floor(pos.y);

        Vector3 pos_ = transform.position;

        pos_.x = MathFast.Floor(pos_.x);
        pos_.y = MathFast.Floor(pos_.y);

        return pos_.x == pos.x && pos_.y == pos.y;
    }
    private Vector3 GetPoint()
    {
        Vector3 pos = cameraMain.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        pos.z = 0;

        return pos;
    }
}
