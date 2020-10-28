using LeopotamGroup.Math;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSelector : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private SpriteRenderer selectorSprite;
    [Header("Другое")]
    [SerializeField] private BlockData blockToSet;
    [SerializeField, HideInInspector] private Vector3 onWorldPos;

    [SerializeField] private EventSystem sys;
    [SerializeField] private Inventory inventory;

    //Destroying Block
    [Header("Уничтожение блока")]
    [SerializeField] private Sprite[] destroyingStadies;
    [SerializeField] private SpriteRenderer destroyingProcess;

    private bool _isDeleting;
    private Vector3 _posBlockDelete;
    private float _currentTime;
    private float _blockToDeleteTime;
    private float _procent;//
    private ChunkUnit _chunkUnitClick;//Чанк на который нажали
    private float _powerDig;//Мощность всапывания
    
    [SerializeField] private float powerDigDefault;
    private BlockLayer _layerBlockToDelete;
    
    private List<string> _layers;

    private bool updateBlockPos;
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
        _layers = new List<string>();
        _layers.Add("Entity");
        _layers.Add("Item");

        _powerDig = powerDigDefault;
    }
    private void Update()
    {
        if (!_isDeleting)
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
        _currentTime += Time.deltaTime;

        SetSprite();
        if (_currentTime > _blockToDeleteTime)
        {
            DeletingBlock();
        }
    }
    private void SetSprite()
    {
        _procent = _currentTime / _blockToDeleteTime * 100;
        //Debug.Log(procent);
        float m = 0;
        for (int i = 0; i < 10; i++)
        {
            m += 10f;//12.5f;
            if (_procent <= m)
            {
                destroyingProcess.sprite = destroyingStadies[i];
                return;
            }        
        }
    }
    private void DeletingReset()
    {
        _currentTime = 0;
        _blockToDeleteTime = 0;
        
        _isDeleting = false;
        _procent = 0;

        destroyingProcess.sprite = destroyingStadies[0];
        destroyingProcess.enabled = false;
    }
    private void DeletingBlock()
    {
        _chunkUnitClick.DeleteBlock(_posBlockDelete, _layerBlockToDelete);    
        DeletingReset();
    }

    private void StartDeleting(BlockLayer layer = BlockLayer.Front)
    {
        if (!_isDeleting)// && updateBlockPos)
        {
            //updateBlockPos = false;
            _chunkUnitClick = chunkManager.GetChunk(onWorldPos);
            if (_chunkUnitClick.CanBreakBlock(onWorldPos, layer))
            {
                _posBlockDelete = onWorldPos;
                _layerBlockToDelete = layer;
                float hp = _chunkUnitClick?.
                    GetBlockUnit(onWorldPos, layer)?
                    .Data.hp ?? -1;
                
                if (hp != -1)
                {
                    if (hp < 0)
                    {
                        DeletingBlock();
                    }
                    else
                    {
                        _blockToDeleteTime = hp / _powerDig;
                        destroyingProcess.enabled = true;
                        _isDeleting = true;
                        //Debug.Log("StartDeleting");
                    }                   
                }           
            }
        }
    }
    #endregion

    private void ClickPlaceBlock(bool isBack = false)
    {
        var chunk = chunkManager.GetChunk(onWorldPos);
        
        var layer = isBack ? BlockLayer.Back : BlockLayer.Front;
        var blockInteractable = chunk.GetBlockUnit(onWorldPos, layer);
        if (blockInteractable != null && blockInteractable.Data.isInteractable)
        {
            //Debug.Log();
            if (blockInteractable.Data.nameBlock == "Chest")
            {
                OpenChest(blockInteractable.Memory as ChestMemory);
            }
            
            //Debug.Log("Interactable Block Clicked");
            return;
        }
        var item = inventory.GetSelectedItem();
        if (item != null && item.data != null && item.data.type == ItemType.Block)
        {
            if (!CheckCollisions() || isBack)
            {
                var pos = chunk.tilemapFrontWorld.WorldToCell(onWorldPos);
                
                
                if (item.data.block.mustHaveDownerBlock && chunk.GetDownerBlockUnit(new Vector2Int(pos.x, pos.y), layer)==null)
                    return;
                
                if (chunk.SetBlock(pos , item.data.block, true, layer, !isBack))
                {
                    //Debug.Log(_pos);
                    if (!isBack)
                    {
                        chunk.chunkBuilder.RefreshDownerGrassBlock(pos.x, pos.y);
                    }
                    item.RemoveItem();
                }               
            }          
        }       
    }

    private bool CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        var pos = transform.position;
        pos.x -= 0.5f;
        pos.y -= 0.5f;
        var hits = new List<RaycastHit2D[]>();
        hits.Add(Physics2D.RaycastAll(pos, Vector2.right, 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x + 1, pos.y), Vector2.up, 1));
        hits.Add(Physics2D.RaycastAll(pos, Vector2.up, 1));
        hits.Add(Physics2D.RaycastAll(pos, new Vector2(0.5f, 0.5f), 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x + 1f, pos.y), new Vector2(-0.5f, 0.5f), 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x, pos.y + 0.1f), Vector2.right, 1));
        hits.Add(Physics2D.RaycastAll(new Vector3(pos.x, pos.y + 1), Vector2.right, 1));

        for (var i = 0; i < hits.Count; i++)
        {
            for (var z = 0; z < hits[i].Length; z++)
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
                        if (layer == _layers[j])
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
        if (Input.GetMouseButtonUp(0))
        {
            updateBlockPos = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0))
        {
            StartDeleting(BlockLayer.Back);
            return;
        }
        if (Input.GetMouseButton(0))
        {
            StartDeleting();
            return;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1))
        {
            ClickPlaceBlock(true);
            return;
        }

        if (Input.GetMouseButton(1))
        {
            ClickPlaceBlock();
            return;
        }
    }    

    private void AlignPositionGrid(Vector3 pos)
    {
        pos.x = MathFast.Floor(pos.x) + 0.5f;
        pos.y = MathFast.Floor(pos.y) + 0.5f;
        pos.z = 0;

        transform.position = pos;
        
        var block = chunkManager.GetChunk(onWorldPos)?.GetBlockUnit(onWorldPos, BlockLayer.Front);
        if (block != null && block.Data != null && block.Data.isInteractable)
        {
            selectorSprite.color = Color.yellow;
        }
        else
        {
            selectorSprite.color = Color.white;
        }
    }
    private bool InBounds(Vector3 pos)
    {       
        pos.x = MathFast.Floor(pos.x);
        pos.y = MathFast.Floor(pos.y);

        var pos_ = transform.position;

        pos_.x = MathFast.Floor(pos_.x);
        pos_.y = MathFast.Floor(pos_.y);

        return pos_.x == pos.x && pos_.y == pos.y;
    }
    private Vector3 GetPoint()
    {
        var pos = cameraMain.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        pos.z = 0;

        return pos;
    }

    public void CheatPowerDick(float power)
    {
        if (_powerDig == power)
        {
            _powerDig = powerDigDefault;
            return;
        }

        _powerDig = power;
    }

    private void OpenChest(ChestMemory chestMemory)
    {
        inventory.OpenChest(chestMemory);
    }
}
