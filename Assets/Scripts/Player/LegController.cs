using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    private float legExtendedLenght;
    private float extendSpeed;
    private float retractSpeed;

    private float legExtendAmount = 0;

    private Vector2 legDirection;

    private bool isExtending;
    private bool isRetracting;
    
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
        if (isExtending)
        {
            float lenght = Mathf.Lerp(0, legExtendedLenght, legExtendAmount);
            
            transform.localScale = new Vector3(lenght, transform.localScale.y, transform.localScale.z);
            legExtendAmount += extendSpeed * Time.deltaTime;
            
            if (legExtendAmount >= 1) isExtending = false;
        }

        if (isRetracting)
        {
            float lenght = Mathf.Lerp(0, legExtendedLenght, legExtendAmount);
            transform.localScale = new Vector3(lenght, transform.localScale.y, transform.localScale.z);
            legExtendAmount -= retractSpeed * Time.deltaTime;
            if (legExtendAmount <= 0) isExtending = false;
        }
    }
}
