using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class ItemUnit : MonoBehaviour
{
    [HideInInspector]public ItemData.Data data;
    [Range(0f, 1f)]
    [SerializeField] private float scale = 0.25f;
    private void Awake()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = data.sprite;
        sprite.size = new Vector2(scale, scale);
    }
}
