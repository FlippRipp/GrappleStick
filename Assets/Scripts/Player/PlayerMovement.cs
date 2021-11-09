using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private PlayerInput playerInput;

    [Header("Grapple")]
    [SerializeField]
    private GameObject grappleHookPrefab;

    [SerializeField, Range(0, 50)]
    private float grappleHookProjectileSpeed = 10;
    [SerializeField, Range(0, 50)]
    private float grappleReelSpeed = 2;
    
    

    private bool isGrappling = false;

    private GrappleHook grappleHook;

    private Rigidbody2D rigidBody;

    [SerializeField]
    private GameObject grappleJointPrefab;

    private GrappleJoint grappleJoint;

    private List<Vector2> grappleRopePoint = new List<Vector2>();

    private Vector2 reelForce;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        GrapplingHockInput();
        GrappleMovement();
    }

    /// <summary>
    /// >Reads Input and Sends raycast for grapple hock
    /// </summary>
    void GrapplingHockInput()
    {
        if (playerInput.rightMouseButtonPressedDown)
        {
            GrappleFire();
        }

        if (playerInput.rightMouseButtonPressedUp)
        {
            GrappleRetract();
        }
        
    }

    private void GrappleMovement()
    {
        if (!isGrappling) return;

        //transform.up = Vector3.Lerp(transform.up, (grappleJoint.transform.position - transform.position).normalized, 0.1f);
        grappleJoint.ChangeDistance(playerInput.vertical * grappleReelSpeed * Time.deltaTime);

        reelForce = grappleReelSpeed * playerInput.vertical
                                     * (grappleJoint.transform.position - transform.position).normalized;


    }

    private void GrappleFire()
    {
        if (isGrappling)
        {
            GrappleRetract();
        }
        
        Vector2 mousePosition = Vector2.zero;
            
        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
             new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            
        Vector2 moveDirection = (mousePosition - (Vector2)transform.position).normalized;
        
        if (!grappleHook)
        {
            grappleHook =
                Instantiate(grappleHookPrefab, transform.position, Quaternion.identity).GetComponent<GrappleHook>();
        }
        else
        {
            grappleHook.transform.position = transform.position;
        }
        
        
        grappleHook.InitGrapple(moveDirection, grappleHookProjectileSpeed, this);

    }

    private void GrappleRetract()
    {
        if (isGrappling)
        {
            rigidBody.velocity += reelForce;
            reelForce = Vector2.zero;
            grappleJoint.ClearAttachment();
            grappleRopePoint.Clear();
            isGrappling = false;
        }
        grappleHook.gameObject.SetActive(false);
    }

    public void OnGrappleHit(Vector2 hitPoint)
    {
        isGrappling = true;
        grappleRopePoint.Add(hitPoint);

        if (!grappleJoint)
        {
            grappleJoint = Instantiate(grappleJointPrefab, hitPoint, quaternion.identity)
                .GetComponent<GrappleJoint>();
        }
        else
        {
            grappleJoint.transform.position = hitPoint;
        }

        grappleJoint.SetUpAttachment(rigidBody);

    }
}

