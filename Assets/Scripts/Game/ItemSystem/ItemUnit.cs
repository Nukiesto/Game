using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemUnit : MonoBehaviour
{
    [HideInInspector] public ItemData.Data data;
    [HideInInspector] public SpriteRenderer sprite;

    [SerializeField] private string[] tagList; // фильтр по тегам
    [SerializeField] private LayerMask layers; // или по слоям
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void InitSprite()
    {
        Sprite spr = data.sprite;
        if (spr != null)
        {
            sprite.sprite = spr;
        }
    }

    private bool Check(GameObject obj)
    {
        //if (((1 << obj.layer) & layers) != 0)
        //{
        //    return true;
        //}

        foreach (string t in tagList)
        {
            if (obj.CompareTag(t)) return true;
        }

        return false;
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    foreach (ContactPoint2D contact in collision.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal, Color.white);
    //    }
    //    //Debug.Log(collision.gameObject);
    //    if (Check(collision.gameObject))
    //    {
    //        //Debug.Log("True");
    //        GetComponent<PoolObject>().ReturnToPool();
    //    }
    //    //Debug.Log("Trueasfsfa");
    //}   
}
