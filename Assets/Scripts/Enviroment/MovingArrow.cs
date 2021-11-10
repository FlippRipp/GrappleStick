using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingArrow : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveDelta;
    private Rigidbody2D rigidbody;
    public LayerMask grappleLayers;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = moveDelta;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (grappleLayers == (grappleLayers | (1 << collision.gameObject.layer)))
        {
        Destroy(gameObject);
        }
    }
}

