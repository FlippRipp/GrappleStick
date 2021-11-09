using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleJoint : MonoBehaviour
{
    [SerializeField]
    private DistanceJoint2D grappleDistanceJoint;
    [SerializeField]
    private HingeJoint2D grappleHingeJoint;
    
    

    public void ChangeDistance(float distanceDelta)
    {
        float dist = Vector3.Distance(grappleDistanceJoint.connectedBody.position, transform.position);
        
        if (distanceDelta > 0 && grappleDistanceJoint.distance >
            Vector3.Distance(grappleDistanceJoint.connectedBody.position, transform.position))
        {
            grappleDistanceJoint.distance = Mathf.Lerp(grappleDistanceJoint.distance, dist, 0.2f);
        }

        grappleDistanceJoint.distance -= distanceDelta;
        
        grappleDistanceJoint.distance = Mathf.Max(grappleDistanceJoint.distance, 1);
    }

    public void SetDistance(float distance)
    {
        grappleDistanceJoint.distance = Mathf.Max(1, distance);
    }

    public void SetUpAttachment(Rigidbody2D rigidbodyToAttach)
    {
        SetDistance(Vector3.Distance(rigidbodyToAttach.position, transform.position));
        
        grappleDistanceJoint.connectedBody = rigidbodyToAttach;
    }

    public void ClearAttachment()
    {
        grappleDistanceJoint.connectedBody = null;
    }
}
