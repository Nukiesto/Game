using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Occlusion : MonoBehaviour
{
    [SerializeField]private GameObject prefab;
    [SerializeField, HideInInspector] private GameObject content;

    private void Start()
    {
        content = Instantiate(prefab);
        content.transform.parent = transform;
        content.transform.position = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OcclusionCollider"))
            content.SetActive(true);
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("OcclusionCollider"))
            content.SetActive(false);//!content.activeSelf);
    }
}