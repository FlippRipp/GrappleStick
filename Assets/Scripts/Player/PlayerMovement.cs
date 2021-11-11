using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public partial class PlayerMovement : MonoBehaviour
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
    private float legMinLenght = 2;
    [SerializeField]
    private float legForce = 1;

    [SerializeField]
    private float legChargeMaxTime = 3;
    [SerializeField]
    private float legExtendSpeed = 2;
    [SerializeField]
    private float legRetractSpeed = 2;
    [SerializeField]
    private LayerMask legCollisionMask;

    [SerializeField] private UnityEvent<float> onFootCharge;

    [SerializeField] private float legMinShakeMagnitude = 0f;
    [SerializeField] private float legMaxShakeMagnitude = 0.2f;
    [SerializeField] private float legMinShakeFrequency = 0.3f;
    [SerializeField] private float legMaxShakeFrequency = 0.3f;
    

    private bool legCharging;
    private float legChargeTime;

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
        LegUpdate();
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
            
        legDirection = -(mousePosition - (Vector2)transform.position).normalized;
    }

    private void LegUpdate()
    {
        LegRotation();

        if (legCharging)
        {
            onFootCharge.Invoke(legChargeTime / legChargeMaxTime);
            legController.ChangeLegShakeFrequency(Mathf.Lerp(legMinShakeFrequency, legMaxShakeFrequency,
                legChargeTime / legChargeMaxTime));
            
            legController.ChangeLegShakeMagnitude(Mathf.Lerp(legMinShakeMagnitude, legMaxShakeMagnitude,
                legChargeTime / legChargeMaxTime));
            
            legChargeTime += Time.deltaTime;
            legChargeTime = Mathf.Min(legChargeTime, legChargeMaxTime);
        }
        else
        {
            legController.SetLegDirection(legDirection);

        }
        
        if (playerInput.leftMouseButtonPressedDown)
        {
            legController.SetLegDirection(legDirection);
            legController.StartLegShake(0, legMinShakeFrequency);
            legCharging = true;
        }

        if (playerInput.leftMouseButtonPressedUp)
        {
            onFootCharge.Invoke(0);
            legController.EndLegShake();
            ExtendLeg();
            //legController.RetractLeg(legRetractSpeed);
            legCharging = false;
            legChargeTime = 0;
        }
    }

    private void ExtendLeg()
    {
        LegRotation();
        float distance = Mathf.Max(legMinLenght, legMaxLenght * (legChargeTime / legChargeMaxTime));

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, legDirection,
                legMaxLenght * (legChargeTime / legChargeMaxTime), legCollisionMask);

        if (hit2D.collider)
        {
            distance = hit2D.distance;
            rigidBody.AddForce(-legDirection * (legForce * (legChargeTime / legChargeMaxTime)), ForceMode2D.Impulse);
        }

        distance = Mathf.Min(distance, legMaxLenght);
        legController.ExtendLeg(distance, legExtendSpeed / distance * (legChargeTime / legChargeMaxTime),
            legDirection);
    }

    private void GrappleMovement()
    {
        if (!isGrappling) return;

        RopeCut();

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

