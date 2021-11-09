using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    private float grappleSpeed = 0;
    private Vector2 movementDirection;

    [SerializeField]
    private LayerMask grappleLayers;

    private Rigidbody2D rigidBody;
    private PlayerMovement playerMovement;

    private bool hasHitSurface;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void InitGrapple(Vector2 moveDir, float speed, PlayerMovement playerM)
    {
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        hasHitSurface = false;
        grappleSpeed = speed;
        movementDirection = moveDir;
        playerMovement = playerM;
        rigidBody.velocity = moveDir * grappleSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(hasHitSurface) return;
        if (grappleLayers == (grappleLayers | (1 << other.gameObject.layer)))
        {
            hasHitSurface = true;
            //might not allow it to move at all?
            rigidBody.bodyType = RigidbodyType2D.Static;
            
            playerMovement.OnGrappleHit(other.GetContact(0).point);
            
        }
    }
}
