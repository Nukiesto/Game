using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x + h * speed, pos.y + v * speed);

        //Vector2 force = new Vector2(h * speed,  v * speed);

        //rigidbody.AddForce(force, ForceMode2D.Force);
        
        
        //rigidbody.AddForce(force, ForceMode2D.Impulse);
    }
}
