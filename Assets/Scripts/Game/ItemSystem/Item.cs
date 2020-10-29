using System;
using Singleton;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    public ItemData.Data data;
    [HideInInspector] public SpriteRenderer sprite;
    public ItemManager itemManager;
    
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void InitSprite()
    {
        var spr = data?.sprite;
        if (spr != null)
        {
            sprite.sprite = spr;
        }
    }

    private void OnDisable()
    {
        itemManager.RemoveItem(this);
    }
}
