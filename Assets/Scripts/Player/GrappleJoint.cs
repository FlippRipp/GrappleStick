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
        grappleDistanceJoint.distance -= distanceDelta;
        grappleDistanceJoint.distance = Mathf.Max(grappleDistanceJoint.distance, 1);
    }

    public void SetDistance(float distance)
    {
        grappleDistanceJoint.distance = Mathf.Max(1, distance);
    }

    public void SetUpAttachment(Rigidbody2D rigidbodyToAttach)
    {
        grappleDistanceJoint.connectedBody = rigidbodyToAttach;
    }

    public void ClearAttachment()
    {
        grappleDistanceJoint.connectedBody = null;
    }
}
