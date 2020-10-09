using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CircleCollider2D itemMagnet;
    [SerializeField] private float itemPickRadius;

    [SerializeField] private Inventory inventory;
    void Start()
    {
        itemMagnet.radius = itemPickRadius;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Item(Clone)")
        {
            GameObject obj = col.gameObject;
            //Debug.Log(obj.GetComponent<Item>().data);
            inventory.AddItem(obj.GetComponent<Item>().data);
            obj.SetActive(false);
        }
    }
}
