using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [HideInInspector] public ItemData.Data data;
    [HideInInspector] public SpriteRenderer sprite;

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
}
