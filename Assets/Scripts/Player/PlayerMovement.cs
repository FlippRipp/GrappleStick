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
    private float grappleHookSpeed = 10;

    private bool isGrappling = false;

    private GrappleHook grappleGameObject;

    private Rigidbody2D rigidBody;

    [SerializeField]
    private HingeJoint2D grappleJointPrefab;

    private HingeJoint2D grappleJoint;

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

    private void GrappleFire()
    {
        if(isGrappling) return;
        isGrappling = true;
        Vector2 mousePosition = Vector2.zero;
            
        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
             new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            
        Vector2 moveDirection = (mousePosition - (Vector2)transform.position).normalized;
        
        Debug.Log("MousePos: " + mousePosition + " Dir: " + moveDirection);

        if (!grappleGameObject)
        {
            grappleGameObject =
                Instantiate(grappleHookPrefab, transform.position, Quaternion.identity).GetComponent<GrappleHook>();
        }
        
        grappleGameObject.InitGrapple(moveDirection, grappleHookSpeed, this);

    }

    public void OnGrappleHit(Vector2 hitPoint)
    {
        grappleRopePoint.Add(hitPoint);

        if (!grappleJoint)
        {
            grappleJoint = Instantiate(grappleJointPrefab, hitPoint, quaternion.identity)
                .GetComponent<HingeJoint2D>();
        }
        else
        {
            grappleJoint.transform.position = hitPoint;
        }

        grappleJoint.connectedBody = rigidBody;

    }
}

