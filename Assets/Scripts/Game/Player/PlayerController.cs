using Game.ChunkSystem;
using Game.ItemSystem;
using Game.UI;
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
    [SerializeField] private PlayerMovement playerMovement;

    private bool _canInput;
    private bool _flashLightActive;
    public static PlayerController Instance;
    
    private void Start()
    {
        Instance = this;
        itemMagnet.radius = itemPickRadius;
        
        var worldManager = Toolbox.Instance.mWorldManager;
        if (worldManager.TryGetLoadedPoint(out var loadedPoint))
        {
            transform.position = loadedPoint;
            Debug.Log(loadedPoint);
            //Debug.Log("PlayerLoadPoint");
        }
        else
        {
            transform.position = worldManager.SpawnPoint;
            Debug.Log(worldManager.SpawnPoint);
            //Debug.Log("PlayerSpawnPoint");
        }
        worldManager.MoveCameraToPoint(transform.position);

        Console.OnToggleConsoleEvent += SetCanInput;
    }

    private void Update()
    {
        if (!_canInput) return;
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            _flashLightActive = !_flashLightActive;
            flashLight.SetActive(_flashLightActive);
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name);
        if (!col.gameObject.CompareTag("Item")) return;
        
        var obj = col.gameObject;
        //Debug.Log(obj.GetComponent<Item>().data);
        inventory.AddItem(obj.GetComponent<Item>().data);
        obj.SetActive(false);
    }
    public bool CanToCreateItem()
    {
        var pos = itemCreatePos.transform.position;
        return !chunkManager.GetChunk(pos).HasBlock(pos);
    }
    public void CreateItemKick(ItemData.Data data, int count)
    {
        var pos = itemCreatePos.transform.position;

        var itemManager = Toolbox.Instance.mItemManager;

        for (var i = 0; i < count; i++)
        {
            itemManager.CreateItem(pos, data);
        }       
    }

    private void SetCanInput(bool value)
    {
        _canInput = value;
        playerMovement.SetCanMove(value);
    }
    
}
