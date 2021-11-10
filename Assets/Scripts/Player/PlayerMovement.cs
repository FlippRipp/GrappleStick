using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private PlayerInput playerInput;
    private Rigidbody2D rigidBody;

    [Header("Grapple")]
    [SerializeField]
    private GameObject grappleHookPrefab;

    [SerializeField, Range(0, 50)]
    private float grappleHookProjectileSpeed = 10;
    [SerializeField, Range(0, 50)]
    private float grappleReelSpeed = 2;
    [SerializeField]
    private GameObject grappleJointPrefab;
    
    private bool isGrappling = false;
    private GrappleHook grappleHook;
    private GrappleJoint grappleJoint;
    private List<Vector2> grappleRopePoint = new List<Vector2>();
    
    private Vector2 reelForce;


    [Space]
    [Header("Leg")]
    [SerializeField]
    private LegController legController;
    [SerializeField]
    private float legMaxLenght = 2;
    [SerializeField]
    private float legJumpForce = 1;
    [SerializeField]
    private float legExtendSpeed = 2;
    [SerializeField]
    private float legRetractSpeed = 2;
    [SerializeField]
    private LayerMask legCollisionMask;

    private Vector2 legDirection;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        GrapplingHockInputs();
        GrappleMovement();
        LegInputs();
    }

    /// <summary>
    /// >Reads Input and Sends raycast for grapple hock
    /// </summary>
    private void GrapplingHockInputs()
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

    private void LegRotation()
    {
        Vector2 mousePosition = Vector2.zero;
            
        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            
        legDirection = (mousePosition - (Vector2)transform.position).normalized;
        
        //legController.UpdateLegDirection(legDirection);
    }

    private void LegInputs()
    {
        if (playerInput.leftMouseButtonPressedDown)
        {
            ExtendLeg();
        }

        if (playerInput.leftMouseButtonPressedUp)
        {
            legController.RetractLeg(legRetractSpeed);
        }
    }

    private void ExtendLeg()
    {
        LegRotation();
        RaycastHit2D hit2D
            = Physics2D.Raycast(transform.position, legDirection, legMaxLenght, legCollisionMask);

        float distance = legMaxLenght;
        if (hit2D.collider)
        {
            distance = hit2D.distance;
            rigidBody.AddForce(-legDirection * legJumpForce, ForceMode2D.Impulse);
        }

        distance = Mathf.Min(distance, legMaxLenght);
        legController.ExtendLeg(distance, legExtendSpeed / distance, legDirection);


    }

    private void GrappleMovement()
    {
        if (!isGrappling) return;

        RopeCut();

        //transform.up = Vector3.Lerp(transform.up, (grappleJoint.transform.position - transform.position).normalized, 0.1f);
        grappleJoint.ChangeDistance(playerInput.vertical * grappleReelSpeed * Time.deltaTime);

        reelForce = grappleReelSpeed * playerInput.vertical
                                     * (grappleJoint.transform.position - transform.position).normalized;
    }

    private Vector2[] vertices = new Vector2[64];

    private void RopeCut()
    {
        if (grappleHook && grappleHook.gameObject.activeInHierarchy)
        {
            Vector2 pos = grappleHook.transform.position;

            Vector2 delta = -((Vector2)pos - (Vector2)transform.position);

            RaycastHit2D ray = Physics2D.Raycast(pos, delta.normalized, delta.magnitude, grappleHook.grappleLayers);

            if (ray.collider)
            {
                CompositeCollider2D cc2d = ray.collider as CompositeCollider2D;
                if (cc2d)
                {
                    Vector2 point = Vector2.positiveInfinity;

                    float currentLength = Mathf.Infinity;

                    Vector3 meanPos = (ray.point);

                    for (int i = 0; i < cc2d.pathCount; i++)
                    {
                        for (int ii = 0; ii < cc2d.GetPath(i, vertices); ii++)
                        {
                            float thisLength = Vector2.SqrMagnitude((Vector2)meanPos - vertices[ii]);
                            if (thisLength < currentLength)
                            {
                                currentLength = thisLength;
                                point = vertices[ii];
                            }
                        }
                    }

                    Debug.DrawLine(grappleHook.transform.position, point, Color.red);
                    Debug.DrawLine(point, transform.position, Color.red);
                }

            }
            else
            {
                Debug.DrawLine(grappleHook.transform.position, transform.position, Color.red);
            }
        }

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

    public void GrappleRetract()
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<MovingArrow>())
        {
            GrappleRetract();
        }
    }
}

