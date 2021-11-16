using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField]
    private Portal portal1;
    [SerializeField]
    private Portal portal2;

    private float portalCooldownTime = 1;
    private float portalTimer;

    private bool portalOnCooldown = false;
    private Portal lastExitPortal;

    private void Awake()
    {
        portal1.portalController = this;
        portal2.portalController = this;
    }

    private void Update()
    {
        if (portalOnCooldown)
        {
            portalTimer += Time.deltaTime;
            if (portalTimer > portalCooldownTime)
            {
                portalTimer = 0;
                portalOnCooldown = false;
            }
        }
    }

    public void Teleport(Portal enterPortal, Rigidbody2D objectToTeleport)
    {
        if(portalOnCooldown && enterPortal == lastExitPortal) return;

        Portal exitPortal = (enterPortal == portal1) ? portal2 : portal1;

        Vector2 localEnterPoint = enterPortal.transform.InverseTransformPoint(objectToTeleport.position);
        Vector2 globalExitPos = exitPortal.transform.TransformPoint(localEnterPoint);
        
        Vector2 localVelocity = enterPortal.transform.InverseTransformDirection(objectToTeleport.velocity);
        Vector2 globalExitVelocity = exitPortal.transform.TransformDirection(localVelocity);

        Debug.Log("A: " + objectToTeleport.transform.position + " b: " + globalExitPos);

        objectToTeleport.transform.position = globalExitPos;
        objectToTeleport.velocity = -globalExitVelocity;
        lastExitPortal = exitPortal;
        

        portalOnCooldown = true;
    }
}
