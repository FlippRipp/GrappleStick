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
    [SerializeField] private GameObject grappleHookPrefab;

    [SerializeField, Range(0, 50)] private float grappleHookProjectileSpeed = 10;
    [SerializeField, Range(0, 50)] private float grappleReelSpeed = 2;
    [SerializeField] private GameObject grappleJointPrefab;
    
    private bool isGrappling = false;
    private GrappleHook grappleHook;
    [HideInInspector] public GrappleJoint grappleJoint;
    [HideInInspector] public List<Vector2> grappleRopePoint = new List<Vector2>();
    
    private Vector2 reelForce;


    [Space]
    [Header("Leg")]
    [SerializeField] private LegController legController;
    [SerializeField] private float legMaxLenght = 2;
    [SerializeField] private float legMinLenght = 2;
    [SerializeField] private float legForce = 1;
    [SerializeField] private float legMinForce = 1;

    [SerializeField] private float legChargeMaxTime = 3;
    [SerializeField] private float legExtendSpeed = 2;
    [SerializeField] private float legRetractSpeed = 2;
    [SerializeField] private LayerMask legCollisionMask;



    [SerializeField] private float legMinShakeMagnitude = 0f;
    [SerializeField] private float legMaxShakeMagnitude = 0.2f;
    [SerializeField] private float legMinShakeFrequency = 0.3f;
    [SerializeField] private float legMaxShakeFrequency = 0.3f;

    [SerializeField] private bool isLegReversed = false;
    [SerializeField] private bool canAimWhileCharging = false;


    private bool legCharging;
    private float legChargeTime;

    private Vector2 legDirection;
    
    [Header("Health & Damage")]
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;

    [SerializeField] private float maxSize = 1;
    [SerializeField] private float minSize = 0.1f;
    [SerializeField] private float minDamageVelocity = 10;
    [SerializeField] private float maxDamageVelocity = 100;
    [SerializeField] private float minDamage = 10;
    [SerializeField] private float maxDamage = 100;

    [SerializeField] private ParticleSystem minorWoundParticlePrefab;
    [SerializeField, Min(0)] private float velocityToSufferMinorWound = 10;
    [SerializeField, Min(0)] private float minorWoundForce = 0;
    [SerializeField] private ParticleSystem _mediumWoundParticlePrefab;
    [SerializeField, Min(0)] private float velocityToSufferMediumWound = 18;
    [SerializeField, Min(0)] private float mediumWoundForce = 0;
    [SerializeField] private ParticleSystem _majorWoundParticlePrefab;
    [SerializeField, Min(0)] private float velocityToSufferMajorWound = 27;
    [SerializeField, Min(0)] private float majorWoundForce = 0;

    [Space]
    [Header("Events")]
    [SerializeField] private UnityEvent<float> onFootCharge;
    [SerializeField] private UnityEvent<float> onVelocityCharge;
    [SerializeField] private UnityEvent<float, float> onHealthChanged;


    private Vector2 averageVelocity;

    private List<ParticleSystem> minorHurtParticleSystems = new List<ParticleSystem>();
    private List<ParticleSystem> mediumHurtParticleSystems = new List<ParticleSystem>();
    private List<ParticleSystem> majorHurtParticleSystems = new List<ParticleSystem>();

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateSize();

    }

    // Update is called once per frame
    void Update()
    {
        onVelocityCharge.Invoke(rigidBody.velocity.magnitude);
        GrapplingHockInputs();
        GrappleMovement();
        LegUpdate();
        WoundUpdate();
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
            
        if(isLegReversed) legDirection = -(mousePosition - (Vector2)transform.position).normalized;
        
        else legDirection = (mousePosition - (Vector2)transform.position).normalized;

    }

    private void WoundUpdate()
    {
        foreach (ParticleSystem wound in minorHurtParticleSystems)
        {
            rigidBody.AddForce(-(Vector2)wound.transform.forward * minorWoundForce);
        }

        foreach (ParticleSystem wound in mediumHurtParticleSystems)
        {
            rigidBody.AddForce(-(Vector2)wound.transform.forward * mediumWoundForce);
        }

        foreach (ParticleSystem wound in majorHurtParticleSystems)
        {
            rigidBody.AddForce(-(Vector2)wound.transform.forward * majorWoundForce);
        }
    }

    private void LegUpdate()
    {
        if (canAimWhileCharging)
        {
            LegRotation();
            legController.SetLegDirection(legDirection);

        }

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
            LegRotation();
            legController.SetLegDirection(legDirection);

        }
        
        if (playerInput.leftMouseButtonPressedDown)
        {
            LegRotation();
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
        //LegRotation();
        float distance = Mathf.Max(legMinLenght, legMaxLenght * (legChargeTime / legChargeMaxTime));

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, legDirection,
            legMaxLenght * (legChargeTime / legChargeMaxTime), legCollisionMask);

        if (hit2D.collider)
        {
            distance = hit2D.distance;
            rigidBody.AddForce(-legDirection * Mathf.Max(legMinForce,
                legForce * (legChargeTime / legChargeMaxTime)), ForceMode2D.Impulse);
        }

        distance = Mathf.Min(distance, legMaxLenght);
        legController.ExtendLeg(distance, legExtendSpeed / distance * (legChargeTime / legChargeMaxTime),
            legDirection);
    }


    private void GrappleMovement()
    {
        if (!isGrappling) return;

        RopeCut();

        float verticalRealInput = Mathf.Min(playerInput.vertical + Input.GetAxis("Mouse ScrollWheel"));

        grappleJoint.ChangeDistance(verticalRealInput * grappleReelSpeed * Time.deltaTime);

        

        reelForce = grappleReelSpeed * playerInput.vertical
                                     * (grappleJoint.transform.position - transform.position).normalized;

        if (isGrappling)
        {
            //grappleJoint.transform.position = grappleHook.transform.position;

            //grappleJoint.transform.position = grappleHook.grappleRope.AnchorPosition;
            //grappleJoint.GetComponent<Rigidbody2D>().MovePosition(grappleHook.grappleRope.AnchorPosition);
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
        grappleHook.gameObject.transform.parent = null;
        grappleHook.gameObject.SetActive(false);
    }

    public void OnGrappleHit(Vector2 hitPoint)
    {
        isGrappling = true;
        grappleRopePoint.Add(hitPoint);

        if (grappleJoint)
        {
            grappleJoint.ClearAttachment();
            Destroy(grappleJoint.gameObject);
        }
            

        grappleJoint = Instantiate(grappleJointPrefab, hitPoint, quaternion.identity)
            .GetComponent<GrappleJoint>();

        grappleJoint.SetUpAttachment(rigidBody);

    }
    
    private void UpdateSize()
    {
        float size = Mathf.Lerp(minSize, maxSize, currentHealth / maxHealth);
        transform.localScale = new Vector3(size, size, 1);
    }

    public void DamageHealth(float damage)
    {
        currentHealth -= damage;
        Debug.Log(damage);
        if (currentHealth < 0)
        {
            Debug.Log("I'm to lazy to make a death screen so you get a debug instead");
        }
        onHealthChanged.Invoke(currentHealth, maxHealth);
        UpdateSize();
    }

    public void HealPlayer(float healthToHeal)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healthToHeal);
        onHealthChanged.Invoke(currentHealth, maxHealth);
        UpdateSize();
    }

    private void AddWound(Vector2 impactPoint, float impactVelocity)
    {
        ParticleSystem particle;

        Quaternion woundRotation = Quaternion.LookRotation(((Vector3)impactPoint - transform.position).normalized);
        if (impactVelocity >= velocityToSufferMajorWound)
        {
            particle = Instantiate(_majorWoundParticlePrefab, impactPoint, woundRotation, transform);
            majorHurtParticleSystems.Add(particle);
        }
        else if (impactVelocity >= velocityToSufferMediumWound)
        {
            particle = Instantiate(_mediumWoundParticlePrefab, impactPoint, woundRotation, transform);
            mediumHurtParticleSystems.Add(particle);
        }
        else if (impactVelocity >= velocityToSufferMinorWound)
        {
            particle = Instantiate(minorWoundParticlePrefab, impactPoint, woundRotation, transform);
            minorHurtParticleSystems.Add(particle);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<MovingArrow>())
        {
            GrappleRetract();
        }

        
        float impactVelocity = Vector2.Dot(other.relativeVelocity, other.GetContact(0).normal);
        if (impactVelocity > minDamageVelocity)
        {
            
            DamageHealth(Mathf.Lerp(minDamage, maxDamage, (impactVelocity - minDamageVelocity) /
                                                          (maxDamageVelocity - minDamageVelocity)));

            AddWound(other.GetContact(0).point, impactVelocity);
        }
    }
}

