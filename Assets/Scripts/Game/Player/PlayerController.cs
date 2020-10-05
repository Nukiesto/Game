using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float distancePickupItems;
    public RaycastHit2D hit;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.queriesStartInColliders = false;   
        hit = Physics2D.Raycast(transform.position, Vector2.down * transform.localScale.x, distancePickupItems);
        Debug.DrawRay(transform.position, Vector3.down * transform.localScale.x, Color.red, distancePickupItems);
    }
}
