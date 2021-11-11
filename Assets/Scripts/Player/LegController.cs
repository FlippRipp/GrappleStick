using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField]
    private Transform foot;
    
    private float legExtendedLenght;
    private float extendSpeed;
    private float retractSpeed;

    private float legExtendAmount = 0;
    private LineRenderer legRenderer;
    
    Vector2 shakeOffset = Vector2.zero;
    Vector2 shakeTarget = Vector2.zero;
    Vector2 shakeOrigin = Vector2.zero;
    private float legShakeMagnitude = 0;
    private float legShakeFrequency;
    private float shakeLerp;
    private bool isLegShaking;


    private Vector2 legDirection;

    private bool isExtending;
    private bool isRetracting;

    private void Start()
    {
        legRenderer = GetComponent<LineRenderer>();
    }

    public void ExtendLeg(float length, float speed, Vector2 direction)
    {
        legDirection = direction;
        legExtendedLenght = length;
        extendSpeed = speed;
        isExtending = true;
        isRetracting = false;
        legExtendAmount = 0;
    }

    public void ChangeLegShakeMagnitude(float shakeMagnitude)
    {
        legShakeMagnitude = shakeMagnitude;
    }
    public void ChangeLegShakeFrequency(float shakeFrequency)
    {
        legShakeFrequency = shakeFrequency;
    }

    public void StartLegShake(float shakeMagnitude, float shakeFrequency)
    {
        legShakeMagnitude = shakeMagnitude;
        legShakeFrequency = shakeFrequency;

        isLegShaking = true;
    }

    public void EndLegShake()
    {
        isLegShaking = false;
        shakeOffset = Vector2.zero;
    }

    private void LegShake()
    {
        if(!isLegShaking) return;
        
        shakeOffset = Vector2.Lerp(shakeOrigin, shakeTarget, shakeLerp);
        shakeLerp += Time.deltaTime * legShakeFrequency;
        
        if (shakeLerp >= 1)
        {
            shakeLerp = 0;
            shakeOrigin = shakeTarget;
            shakeTarget = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * legShakeMagnitude;
        }
    }

    public void RetractLeg(float speed)
    {
        retractSpeed = speed;
        isExtending = false;
        isRetracting = true;
    }

    public void UpdateLegDirection()
    {
        transform.right = legDirection;
    }

    public void SetLegDirection(Vector2 direction)
    {
        if(isExtending || isRetracting) return;
        legDirection = direction;
    }

    private void Update()
    {
        LegShake();
        UpdateLegDirection();
        legRenderer.SetPosition(0, transform.position);
        legRenderer.SetPosition(1, transform.position);

        foot.position = transform.position + (Vector3)shakeOffset;
        
        if (isExtending)
        {
            float lenght = Mathf.Lerp(0, legExtendedLenght, legExtendAmount);

            
            legExtendAmount += extendSpeed * Time.deltaTime;

            Vector3 legPos = transform.position + (Vector3) legDirection * lenght;
            foot.position = legPos + (Vector3)shakeOffset;
            legRenderer.SetPosition(1, legPos);
            if (legExtendAmount >= 1)
            {
                isExtending = false;
                isRetracting = true;

            }
        }
        
        if (isRetracting)
        {
            float lenght = Mathf.Lerp(0, legExtendedLenght, legExtendAmount);
            legExtendAmount -= extendSpeed * Time.deltaTime;
            
            Vector3 legPos = transform.position + (Vector3) legDirection * lenght;
            foot.position = legPos + (Vector3)shakeOffset;
            legRenderer.SetPosition(1, legPos);
            if (legExtendAmount <= 0) isRetracting = false;
        }
    }
}
