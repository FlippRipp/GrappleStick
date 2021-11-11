using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    [SerializeField]
    private float force;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerMovement pM;
        if(!other.gameObject.GetComponent<PlayerMovement>()) return;
        
        other.gameObject.GetComponent<Rigidbody2D>().
            AddForce(((Vector2)other.transform.position - other.GetContact(0).point).normalized * force);
        
        
    }
}
