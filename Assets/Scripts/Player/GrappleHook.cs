using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{

    [SerializeField]
    public LayerMask grappleLayers;

    [SerializeField]
    private GrappleRope ropePrefab;

    private Rigidbody2D rigidBody;
    private PlayerMovement playerMovement;
    public GrappleRope grappleRope;

    public bool hasHitSurface;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void InitGrapple(Vector2 moveDir, float speed, PlayerMovement playerM)
    {
        gameObject.SetActive(true);
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.simulated = true;
        hasHitSurface = false;
        playerMovement = playerM;
        rigidBody.velocity = moveDir * speed;

        if (!grappleRope && ropePrefab)
        {
            grappleRope = Instantiate(ropePrefab, transform.position, transform.rotation);
            grappleRope.player = playerMovement;
            grappleRope.grappleHook = gameObject;
        }
    }

    private void Update()
    {
        if (hasHitSurface && playerMovement && playerMovement.grappleJoint
            && grappleRope && grappleRope.ropeNodes.Count <= 1)
        {
            playerMovement.grappleJoint.transform.position = transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(hasHitSurface) return;
        if (grappleLayers == (grappleLayers | (1 << other.gameObject.layer)))
        {
            hasHitSurface = true;
            //might not allow it to move at all?
            rigidBody.simulated = false;
            print("oo");
            playerMovement.OnGrappleHit(other.GetContact(0).point);

            transform.parent = other.transform;


        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //transform.parent = null;
        Destroy(grappleRope.gameObject);
    }
}
