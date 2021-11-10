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

    private void Update()
    {
        UpdateLegDirection();
        legRenderer.SetPosition(0, transform.position);
        
        if (isExtending)
        {
            float lenght = Mathf.Lerp(0, legExtendedLenght, legExtendAmount);

            
            legExtendAmount += extendSpeed * Time.deltaTime;

            Vector3 legPos = transform.position + (Vector3) legDirection * lenght;
            foot.position = legPos;
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
            foot.position = legPos;
            legRenderer.SetPosition(1, legPos);
            if (legExtendAmount <= 0) isExtending = false;
        }
    }
}
