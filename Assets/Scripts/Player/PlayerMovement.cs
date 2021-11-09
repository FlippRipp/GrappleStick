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

    private GrappleHook grappleGameObject;

    private Rigidbody2D rigidBody;

    [SerializeField]
    private GameObject grappleJointPrefab;

    private GrappleJoint grappleJoint;

    private List<Vector2> grappleRopePoint = new List<Vector2>();
    
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
    }

    private void GrappleMovement()
    {
        if (!isGrappling) return;

        grappleJoint.ChangeDistance(playerInput.vertical * grappleReelSpeed * Time.deltaTime);
        
        
    }

    private void GrappleFire()
    {
        if (isGrappling)
        {
            grappleJoint.ClearAttachment();
            grappleRopePoint.Clear();
            isGrappling = false;
        }
        Vector2 mousePosition = Vector2.zero;
            
        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
             new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            
        Vector2 moveDirection = (mousePosition - (Vector2)transform.position).normalized;
        
        if (!grappleGameObject)
        {
            grappleGameObject =
                Instantiate(grappleHookPrefab, transform.position, Quaternion.identity).GetComponent<GrappleHook>();
        }
        else
        {
            grappleGameObject.transform.position = transform.position;
        }
        
        
        grappleGameObject.InitGrapple(moveDirection, grappleHookProjectileSpeed, this);

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

